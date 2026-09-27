using ProductionProgress.Application.Common.Exceptions;
using ProductionProgress.Application.Dtos.Users;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Application.Services;

/// <summary>
/// ユーザー管理に関する処理を行います。
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;

    public UserService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
    }

    /// <summary>
    /// 全ユーザーを取得します。
    /// </summary>
    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(ToResponse)
            .ToList();
    }

    /// <summary>
    /// Workerユーザーを新規作成します。
    ///
    /// nullの場合は、同じユーザー名がすでに存在しています。
    /// </summary>
    public async Task<UserResponse> CreateWorkerAsync(
    CreateUserRequest request)
    {
        // ユーザー名の前後の空白を除去します。
        var username = request.Username.Trim();

        // --------------------------------------------------
        // 同じユーザー名がすでに存在するか確認
        // --------------------------------------------------
        var existingUser =
            await _userRepository.GetByUsernameAsync(username);

        if (existingUser is not null)
        {
            // Controllerへnullを返すのではなく、
            // 「データの競合」を表す例外を送出します。
            throw new ConflictException(
                "同じユーザー名がすでに登録されています。");
        }

        var user = new AppUser
        {
            Username = username,
            Role = UserRole.Worker,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // パスワードをハッシュ化して保存します。
        user.PasswordHash =
            _passwordHashService.HashPassword(
                user,
                request.Password);

        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        return ToResponse(user);
    }

    /// <summary>
    /// Workerユーザーの有効・無効を変更します。
    /// </summary>
    public async Task<SetUserActiveResult> SetActiveAsync(
        int id,
        bool isActive)
    {
        var user =
            await _userRepository.GetByIdAsync(id);

        // --------------------------------------------------
        // ユーザーが存在しない
        // --------------------------------------------------
        if (user is null)
        {
            return new SetUserActiveResult
            {
                Status = SetUserActiveStatus.NotFound
            };
        }

        // --------------------------------------------------
        // AdminユーザーはこのAPIでは変更させない
        // --------------------------------------------------
        //
        // このAPIは「AdminがWorkerを管理する」
        // ための機能として扱います。
        //
        // Adminを無効化して管理者が誰もログインできなくなる
        // 事故を防ぎます。
        if (user.Role == UserRole.Admin)
        {
            return new SetUserActiveResult
            {
                Status = SetUserActiveStatus.AdminNotAllowed
            };
        }

        // --------------------------------------------------
        // Workerの有効状態を変更
        // --------------------------------------------------
        user.IsActive = isActive;

        await _userRepository.SaveChangesAsync();

        return new SetUserActiveResult
        {
            Status = SetUserActiveStatus.Success,
            User = ToResponse(user)
        };
    }

    /// <summary>
    /// EntityをAPI返却用DTOへ変換します。
    /// </summary>
    private static UserResponse ToResponse(AppUser user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}