using ProductionProgress.Application.Dtos.Auth;
using ProductionProgress.Application.Interfaces;

namespace ProductionProgress.Application.Services;

/// <summary>
/// ログインなどの認証処理を担当するサービスです。
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// 必要なサービスをDIから受け取ります。
    /// </summary>
    public AuthService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// ユーザー名とパスワードを確認してログインします。
    /// </summary>
    /// <returns>
    /// ログイン成功時はLoginResponse、
    /// 失敗時はnull
    /// </returns>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // ユーザー名からDBのユーザーを検索します。
        var user = await _userRepository
            .GetByUsernameAsync(request.Username);

        // ユーザーが存在しない場合はログイン失敗です。
        if (user is null)
        {
            return null;
        }

        // 無効化されているユーザーはログインできません。
        if (!user.IsActive)
        {
            return null;
        }

        // 入力されたパスワードと、
        // DBに保存されているPasswordHashを比較します。
        var passwordValid =
            _passwordHashService.VerifyPassword(
                user,
                request.Password);

        if (!passwordValid)
        {
            return null;
        }

        // JWTアクセストークンを生成します。
        var token =
            _jwtTokenGenerator.GenerateToken(user);

        // 認証成功
        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            AccessToken = token.AccessToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    /// <summary>
    /// ログイン中ユーザーのパスワードを変更します。
    /// </summary>
    public async Task<ChangePasswordStatus> ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request)
    {
        // --------------------------------------------------
        // JWTから取得したUserIdを使って、
        // DBから現在のユーザーを取得します。
        // --------------------------------------------------
        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return ChangePasswordStatus.UserNotFound;
        }

        // --------------------------------------------------
        // 現在のパスワードが正しいか確認します。
        // --------------------------------------------------
        var passwordValid =
            _passwordHashService.VerifyPassword(
                user,
                request.CurrentPassword);

        if (!passwordValid)
        {
            return ChangePasswordStatus.CurrentPasswordIncorrect;
        }

        // --------------------------------------------------
        // 現在と同じパスワードへの変更は禁止します。
        // --------------------------------------------------
        if (request.CurrentPassword == request.NewPassword)
        {
            return ChangePasswordStatus.SamePassword;
        }

        // --------------------------------------------------
        // 新しいパスワードをハッシュ化します。
        //
        // 平文のNewPasswordをそのまま
        // PasswordHashへ保存してはいけません。
        // --------------------------------------------------
        user.PasswordHash =
            _passwordHashService.HashPassword(
                user,
                request.NewPassword);

        // --------------------------------------------------
        // DBへ変更を保存します。
        // --------------------------------------------------
        await _userRepository.SaveChangesAsync();

        return ChangePasswordStatus.Success;
    }
}