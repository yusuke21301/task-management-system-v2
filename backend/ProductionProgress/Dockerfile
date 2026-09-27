# ============================================================
# 1. ビルド用ステージ
# ============================================================
# .NET 10 SDKが入ったDockerイメージを使用する。
# SDKにはdotnet restore、build、publishなどの開発ツールが含まれる。
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# コンテナ内で作業するフォルダ
WORKDIR /src


# ============================================================
# 2. プロジェクトファイルをコピー
# ============================================================
# 最初にcsprojだけをコピーしてrestoreすることで、
# NuGetパッケージ取得結果をDockerのキャッシュとして利用しやすくする。

COPY ["ProductionProgress.Api/ProductionProgress.Api.csproj", "ProductionProgress.Api/"]
COPY ["ProductionProgress.Application/ProductionProgress.Application.csproj", "ProductionProgress.Application/"]
COPY ["ProductionProgress.Domain/ProductionProgress.Domain.csproj", "ProductionProgress.Domain/"]
COPY ["ProductionProgress.Infrastructure/ProductionProgress.Infrastructure.csproj", "ProductionProgress.Infrastructure/"]


# ============================================================
# 3. NuGetパッケージを復元
# ============================================================
# APIプロジェクトをrestoreすると、
# APIから参照している他プロジェクトについても復元される。
RUN dotnet restore "ProductionProgress.Api/ProductionProgress.Api.csproj"


# ============================================================
# 4. ソースコード全体をコピー
# ============================================================
COPY . .


# APIプロジェクトのフォルダへ移動
WORKDIR "/src/ProductionProgress.Api"


# ============================================================
# 5. Release用にpublish
# ============================================================
# 実行に必要なファイルを /app/publish に出力する。
RUN dotnet publish "ProductionProgress.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# ============================================================
# 6. 実行用ステージ
# ============================================================
# SDK全部ではなく、ASP.NET Core実行環境だけを使用する。
# これにより最終的なDockerイメージを小さくできる。
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app


# ============================================================
# 7. APIが待ち受けるポート
# ============================================================
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


# ============================================================
# 8. ビルド済みファイルをコピー
# ============================================================
COPY --from=build /app/publish .


# ============================================================
# 9. ASP.NET Core APIを起動
# ============================================================
ENTRYPOINT ["dotnet", "ProductionProgress.Api.dll"]