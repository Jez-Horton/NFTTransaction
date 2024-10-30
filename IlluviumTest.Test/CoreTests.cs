using NUnit.Framework;
using System.Linq;
using IlluviumTest.Data;
using IlluviumTest.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

[TestFixture]
public class NFTServiceTests
{
    private NFTService _nftService;
    private Mock<IOutputService> _mockOutputService;
    private ApplicationDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestNFTDatabase")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _mockOutputService = new Mock<IOutputService>();
        _nftService = new NFTService(_dbContext, _mockOutputService.Object);
    }

    [Test]
    public void MintToken_ShouldAddTokenToOwnership()
    {
        _nftService.MintToken("0x0000000000000000000000000000000000000001", "0x0000000000000000000000000000000000002302");

        var nft = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000001");

        Assert.IsNotNull(nft);
        Assert.AreEqual("0x0000000000000000000000000000000000002302", nft.OwnerAddress);
    }

    [Test]
    public void BurnToken_ShouldRemoveTokenFromOwnership()
    {
        _nftService.MintToken("0x0000000000000000000000000000000000000002", "0x0000000000000000000000000000000000000003");
        _nftService.BurnToken("0x0000000000000000000000000000000000000002");

        var nft = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000002");

        Assert.IsNull(nft);
    }

    [Test]
    public void TransferToken_ShouldTransferOwnership()
    {
        _nftService.MintToken("0x0000000000000000000000000000000000000004", "0x0000000000000000000000000000000000000005");
        _nftService.TransferToken("0x0000000000000000000000000000000000000004", "0x0000000000000000000000000000000000000005", "0x0000000000000000000000000000000000000006");

        var nft = _dbContext.NFTs.SingleOrDefault(n => n.TokenId == "0x0000000000000000000000000000000000000004");

        Assert.IsNotNull(nft);
        Assert.AreEqual("0x0000000000000000000000000000000000000006", nft.OwnerAddress);
    }

    [Test]
    public void MintToken_ShouldOutputCorrectMessage()
    {
        _nftService.MintToken("0x0000000000000000000000000000000000000007", "0x0000000000000000000000000000000000000008");

        _mockOutputService.Verify(m => m.Log("Minted token 0x0000000000000000000000000000000000000007 to address 0x0000000000000000000000000000000000000008."), Times.Once);
    }

    [Test]
    public void PrintNFTOwner_ShouldOutputCorrectOwner()
    {
        _nftService.MintToken("0x0000000000000000000000000000000000000009", "0x0000000000000000000000000000000000000010");
        _nftService.PrintNFTOwner("0x0000000000000000000000000000000000000009");

        _mockOutputService.Verify(m => m.Log("Token 0x0000000000000000000000000000000000000009 is owned by 0x0000000000000000000000000000000000000010."), Times.Once);
    }

    [Test]
    public void PrintNFTOwner_ShouldNotifyIfTokenDoesNotExist()
    {
        _nftService.PrintNFTOwner("0x000000000000000000000000000000000000000B");

        _mockOutputService.Verify(m => m.Log("Token 0x000000000000000000000000000000000000000B does not exist."), Times.Once);
    }

    [Test]
    public void PrintWalletNFTs_ShouldShowOwnedNFTs()
    {
        _nftService.MintToken("0x000000000000000000000000000000000000000C", "0x0000000000000000000000000000000000000011");
        _nftService.MintToken("0x000000000000000000000000000000000000000D", "0x0000000000000000000000000000000000000011");

        _nftService.PrintWalletNFTs("0x0000000000000000000000000000000000000011");

        _mockOutputService.Verify(m => m.Log("Wallet 0x0000000000000000000000000000000000000011 owns NFTs: 0x000000000000000000000000000000000000000C, 0x000000000000000000000000000000000000000D"), Times.Once);
    }

    [Test]
    public void ResetState_ShouldClearAllTokens()
    {
        _nftService.MintToken("0x000000000000000000000000000000000000000E", "0x0000000000000000000000000000000000000012");
        _nftService.ResetState();

        var nfts = _dbContext.NFTs.ToList();

        Assert.IsEmpty(nfts);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
