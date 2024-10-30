using NUnit.Framework;
using Moq;
using IlluviumTest.Data;
using IlluviumTest.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

[TestFixture]
public class CommandHandlerTests
{
    private CommandHandler _commandHandler;
    private Mock<IOutputService> _mockOutputService;
    private ApplicationDbContext _dbContext;
    private NFTService _nftService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestNFTDatabase")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _mockOutputService = new Mock<IOutputService>();
        _nftService = new NFTService(_dbContext, _mockOutputService.Object);
        _commandHandler = new CommandHandler(_nftService, _mockOutputService.Object);
    }

    [Test]
    public void ReadInline_ShouldProcessSingleMintTransaction()
    {
        var json = "{\"Type\": \"Mint\", \"TokenId\": \"0x0000000000000000000000000000000000000001\", \"Address\": \"0x0000000000000000000000000000000000002302\"}";

        _commandHandler.ExecuteCommand("--read-inline", json);

        var nft = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000001");
        Assert.IsNotNull(nft);
        Assert.AreEqual("0x0000000000000000000000000000000000002302", nft.OwnerAddress);

        _mockOutputService.Verify(m => m.Log("Minted token 0x0000000000000000000000000000000000000001 to address 0x0000000000000000000000000000000000002302."), Times.Once);
    }

    [Test]
    public void ReadInline_ShouldProcessMultipleTransactions()
    {
        var jsonArray = "[{\"Type\": \"Mint\", \"TokenId\": \"0x0000000000000000000000000000000000000002\", \"Address\": \"0x0000000000000000000000000000000000000003\"}, " +
                        "{\"Type\": \"Burn\", \"TokenId\": \"0x0000000000000000000000000000000000000002\"}, " +
                        "{\"Type\": \"Mint\", \"TokenId\": \"0x0000000000000000000000000000000000000004\", \"Address\": \"0x0000000000000000000000000000000000000005\"}]";

        _commandHandler.ExecuteCommand("--read-inline", jsonArray);

        var nft1 = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000002");
        var nft2 = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000004");

        Assert.IsNull(nft1); // Burned
        Assert.IsNotNull(nft2);
        Assert.AreEqual("0x0000000000000000000000000000000000000005", nft2.OwnerAddress);

        _mockOutputService.Verify(m => m.Log("Minted token 0x0000000000000000000000000000000000000002 to address 0x0000000000000000000000000000000000000003."), Times.Once);
        _mockOutputService.Verify(m => m.Log("Burned token 0x0000000000000000000000000000000000000002."), Times.Once);
        _mockOutputService.Verify(m => m.Log("Minted token 0x0000000000000000000000000000000000000004 to address 0x0000000000000000000000000000000000000005."), Times.Once);
    }

    [Test]
    public void ReadFile_ShouldProcessTransactionsFromFile()
    {
        var filePath = "test_transactions.json";
        var json = "[{\"Type\": \"Mint\", \"TokenId\": \"0x0000000000000000000000000000000000000006\", \"Address\": \"0x0000000000000000000000000000000000000007\"}, " +
                   "{\"Type\": \"Burn\", \"TokenId\": \"0x0000000000000000000000000000000000000006\"}]";
        File.WriteAllText(filePath, json);

        _commandHandler.ExecuteCommand("--read-file", filePath);

        var nft = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000006");
        Assert.IsNull(nft); // Burned

        _mockOutputService.Verify(m => m.Log("Minted token 0x0000000000000000000000000000000000000006 to address 0x0000000000000000000000000000000000000007."), Times.Once);
        _mockOutputService.Verify(m => m.Log("Burned token 0x0000000000000000000000000000000000000006."), Times.Once);

        File.Delete(filePath);
    }

    [Test]
    public void ReadInline_ShouldHandleInvalidJson()
    {
        var invalidJson = "{\"Type\": \"Mint\", \"TokenId\": ";

        _commandHandler.ExecuteCommand("--read-inline", invalidJson);

        _mockOutputService.Verify(m => m.Log(It.Is<string>(s => s.StartsWith("Invalid JSON format"))), Times.Once);
    }

    [Test]
    public void ReadInline_ShouldHandleUnsupportedTransactionType()
    {
        var json = "{\"Type\": \"Unknown\", \"TokenId\": \"0x0000000000000000000000000000000000000008\"}";

        _commandHandler.ExecuteCommand("--read-inline", json);

        _mockOutputService.Verify(m => m.Log("Unsupported transaction type."), Times.Once);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
