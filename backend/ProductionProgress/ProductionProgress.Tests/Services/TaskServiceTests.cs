using Moq;
using ProductionProgress.Application.Dtos;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Application.Services;
using ProductionProgress.Domain.Entities;
using ProductionProgress.Domain.Enums;

namespace ProductionProgress.Tests.Services
{
    /// <summary>
    /// TaskService の単体テスト。
    ///
    /// 実際のPostgreSQLやTaskRepositoryは使用せず、
    /// MoqでRepositoryの偽物を作成してServiceだけをテストする。
    /// </summary>
    public class TaskServiceTests
    {
        /// <summary>
        /// 指定したIDの作業が存在する場合、
        /// TaskDtoとして正しく取得できることを確認する。
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_TaskExists_ReturnsTaskDto()
        {
            // =========================================
            // Arrange
            // テストに必要なデータを準備する
            // =========================================

            // TaskRepositoryの偽物を作成する
            var taskRepositoryMock = new Mock<ITaskRepository>();

            // ProcessRepositoryの偽物を作成する
            var processRepositoryMock = new Mock<IProcessRepository>();

            // Repositoryから取得される想定の工程
            var process = new WorkProcess
            {
                Id = 1,
                ProcessName = "塗装工程",
                IsActive = true
            };

            // Repositoryから取得される想定の作業
            var task = new WorkTask
            {
                Id = 10,
                TaskName = "タンク塗装",
                ProcessId = 1,
                Process = process,
                Status = WorkTaskStatus.NotStarted,
                PlannedDate = new DateOnly(2026, 9, 29),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // GetByIdAsync(10) が呼ばれたら、
            // 上で作った task を返すように設定する
            taskRepositoryMock
                .Setup(repository => repository.GetByIdAsync(10))
                .ReturnsAsync(task);

            // 本物のRepositoryではなく、
            // MockのRepositoryをTaskServiceへ渡す
            var service = new TaskService(
                taskRepositoryMock.Object,
                processRepositoryMock.Object);

            // =========================================
            // Act
            // 実際にテスト対象の処理を実行する
            // =========================================

            var result = await service.GetTaskByIdAsync(10);

            // =========================================
            // Assert
            // 結果が期待どおりか確認する
            // =========================================

            Assert.NotNull(result);

            Assert.Equal(10, result.Id);
            Assert.Equal("タンク塗装", result.TaskName);
            Assert.Equal(1, result.ProcessId);
            Assert.Equal("塗装工程", result.ProcessName);
            Assert.Equal(WorkTaskStatus.NotStarted, result.Status);

            // RepositoryのGetByIdAsync(10)が
            // 1回だけ呼ばれたことも確認する
            taskRepositoryMock.Verify(
                repository => repository.GetByIdAsync(10),
                Times.Once);
        }

        /// <summary>
        /// 指定したIDの作業が存在しない場合、
        /// null が返されることを確認する。
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_TaskDoesNotExist_ReturnsNull()
        {
            // =========================================
            // Arrange
            // =========================================

            // TaskRepositoryの偽物を作成
            var taskRepositoryMock = new Mock<ITaskRepository>();

            // ProcessRepositoryの偽物を作成
            var processRepositoryMock = new Mock<IProcessRepository>();

            // ID = 999 の作業は存在しない想定なので、
            // Repositoryからnullを返すように設定する
            taskRepositoryMock
                .Setup(repository => repository.GetByIdAsync(999))
                .ReturnsAsync((WorkTask?)null);

            // Mockを使ってTaskServiceを作成
            var service = new TaskService(
                taskRepositoryMock.Object,
                processRepositoryMock.Object);

            // =========================================
            // Act
            // =========================================

            var result = await service.GetTaskByIdAsync(999);

            // =========================================
            // Assert
            // =========================================

            // 作業が存在しないためnullになることを確認
            Assert.Null(result);

            // Repositoryが1回だけ呼ばれたことを確認
            taskRepositoryMock.Verify(
                repository => repository.GetByIdAsync(999),
                Times.Once);
        }

        /// <summary>
        /// 有効な工程を指定して作業を登録した場合、
        /// Repositoryに正しい作業データが渡されることを確認する。
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_ValidRequest_AddsTask()
        {
            // =========================================
            // Arrange
            // テストに必要なデータを準備する
            // =========================================

            var taskRepositoryMock = new Mock<ITaskRepository>();
            var processRepositoryMock = new Mock<IProcessRepository>();

            // 登録先として指定する工程
            var process = new WorkProcess
            {
                Id = 1,
                ProcessName = "塗装工程",
                IsActive = true
            };

            // 作業登録リクエスト
            var request = new CreateTaskRequest
            {
                TaskName = "タンク塗装",
                ProcessId = 1,
                Status = WorkTaskStatus.NotStarted,
                PlannedDate = new DateOnly(2026, 10, 1)
            };

            // ProcessId = 1 を検索したら、
            // 有効な「塗装工程」が見つかるようにする
            processRepositoryMock
                .Setup(repository => repository.GetByIdAsync(1))
                .ReturnsAsync(process);

            // AddAsyncに渡されたWorkTaskを、
            // そのまま登録後のWorkTaskとして返すようにする。
            taskRepositoryMock
                .Setup(repository =>
                    repository.AddAsync(It.IsAny<WorkTask>()))
                .ReturnsAsync((WorkTask task) => task);

            var service = new TaskService(
                taskRepositoryMock.Object,
                processRepositoryMock.Object);

            // =========================================
            // Act
            // 作業登録を実行する
            // =========================================

            var result = await service.CreateTaskAsync(request);

            // =========================================
            // Assert
            // =========================================

            Assert.NotNull(result);

            Assert.Equal("タンク塗装", result.TaskName);
            Assert.Equal(1, result.ProcessId);
            Assert.Equal("塗装工程", result.ProcessName);
            Assert.Equal(WorkTaskStatus.NotStarted, result.Status);
            Assert.Equal(new DateOnly(2026, 10, 1), result.PlannedDate);

            // 指定された工程が存在するか確認するため、
            // GetByIdAsync(1) が1回呼ばれたことを確認する
            processRepositoryMock.Verify(
                repository => repository.GetByIdAsync(1),
                Times.Once);

            // TaskRepository.AddAsync() に、
            // 正しい内容のWorkTaskが渡されたことを確認する
            taskRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.Is<WorkTask>(task =>
                        task.TaskName == "タンク塗装" &&
                        task.ProcessId == 1 &&
                        task.Process == process &&
                        task.Status == WorkTaskStatus.NotStarted &&
                        task.PlannedDate == new DateOnly(2026, 10, 1))),
                Times.Once);
        }

        /// <summary>
        /// 存在しない工程IDを指定した場合、
        /// InvalidOperationException が発生することを確認する。
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_ProcessDoesNotExist_ThrowsInvalidOperationException()
        {
            // =========================================
            // Arrange
            // =========================================

            var taskRepositoryMock = new Mock<ITaskRepository>();
            var processRepositoryMock = new Mock<IProcessRepository>();

            var request = new CreateTaskRequest
            {
                TaskName = "タンク塗装",

                // 存在しない工程IDを指定する
                ProcessId = 999,

                Status = WorkTaskStatus.NotStarted,
                PlannedDate = new DateOnly(2026, 10, 1)
            };

            // 工程ID = 999 は存在しない想定なので、
            // Repositoryからnullを返すように設定する。
            processRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(999))
                .ReturnsAsync((WorkProcess?)null);

            var service = new TaskService(
                taskRepositoryMock.Object,
                processRepositoryMock.Object);

            // =========================================
            // Act
            // =========================================

            // CreateTaskAsyncを実行すると
            // InvalidOperationExceptionが発生することを確認する。
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateTaskAsync(request));

            // =========================================
            // Assert
            // =========================================

            // 例外メッセージも期待どおりか確認する。
            Assert.Equal(
                "工程ID 999 は存在しません。",
                exception.Message);

            // 工程存在確認が1回行われたことを確認する。
            processRepositoryMock.Verify(
                repository =>
                    repository.GetByIdAsync(999),
                Times.Once);

            // 工程が存在しない場合、
            // Taskの登録処理まで進んではいけない。
            taskRepositoryMock.Verify(
                repository =>
                    repository.AddAsync(It.IsAny<WorkTask>()),
                Times.Never);
        }
    }
}