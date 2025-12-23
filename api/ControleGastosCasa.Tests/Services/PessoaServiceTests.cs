using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services;
using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Enums;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;
using ControleGastosCasa.Tests.Helpers;
using FluentAssertions;
using Moq;

namespace ControleGastosCasa.Tests.Services;

public class PessoaServiceTests
{
    private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
    private readonly IMapper _mapper;
    private readonly PessoaService _pessoaService;

    public PessoaServiceTests()
    {
        _pessoaRepositoryMock = new Mock<IPessoaRepository>();
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _mapper = TestHelper.CreateMapper();
        _pessoaService = new PessoaService(
            _pessoaRepositoryMock.Object,
            _transacaoRepositoryMock.Object,
            _mapper
        );
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var pessoaDto = new PessoaDto
        {
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var pessoaEntity = new Pessoa
        {
            Id = 1,
            Nome = pessoaDto.Nome,
            DataNascimento = pessoaDto.DataNascimento
        };

        _pessoaRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()))
            .Callback<Pessoa, CancellationToken>((p, ct) => { p.Id = pessoaEntity.Id; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _pessoaService.CreateAsync(pessoaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Nome.Should().Be(pessoaDto.Nome);
        result.Data.DataNascimento.Should().Be(pessoaDto.DataNascimento);
        
        _pessoaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComErroNoRepository_DeveRetornarFalha()
    {
        // Arrange
        var pessoaDto = new PessoaDto
        {
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _pessoaRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro no banco de dados"));

        // Act
        var result = await _pessoaService.CreateAsync(pessoaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Ocorreu um erro ao criar a pessoa");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarPessoa()
    {
        // Arrange
        var pessoaId = 1;
        var pessoaEntity = new Pessoa
        {
            Id = pessoaId,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoaEntity);

        // Act
        var result = await _pessoaService.GetByIdAsync(pessoaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(pessoaId);
        result.Data.Nome.Should().Be(pessoaEntity.Nome);
        
        _pessoaRepositoryMock.Verify(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var pessoaId = 999;

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var result = await _pessoaService.GetByIdAsync(pessoaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Pessoa não encontrada");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarPessoa()
    {
        // Arrange
        var pessoaId = 1;
        var pessoaExistente = new Pessoa
        {
            Id = pessoaId,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var pessoaDtoAtualizado = new PessoaDto
        {
            Nome = "João Silva Santos",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoaExistente);

        _pessoaRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _pessoaService.UpdateAsync(pessoaId, pessoaDtoAtualizado);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Nome.Should().Be(pessoaDtoAtualizado.Nome);
        pessoaExistente.Nome.Should().Be(pessoaDtoAtualizado.Nome);
        
        _pessoaRepositoryMock.Verify(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()), Times.Once);
        _pessoaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var pessoaId = 999;
        var pessoaDto = new PessoaDto
        {
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var result = await _pessoaService.UpdateAsync(pessoaId, pessoaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Pessoa não encontrada");
        
        _pessoaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ComIdExistente_DeveDeletarPessoa()
    {
        // Arrange
        var pessoaId = 1;
        var pessoaEntity = new Pessoa
        {
            Id = pessoaId,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoaEntity);

        _pessoaRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _pessoaService.DeleteAsync(pessoaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        
        _pessoaRepositoryMock.Verify(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()), Times.Once);
        _pessoaRepositoryMock.Verify(x => x.DeleteAsync(pessoaEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var pessoaId = 999;

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var result = await _pessoaService.DeleteAsync(pessoaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Pessoa não encontrada");
        
        _pessoaRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_SemFiltro_DeveRetornarTodasPessoas()
    {
        // Arrange
        var pessoas = new List<Pessoa>
        {
            new Pessoa { Id = 1, Nome = "João Silva", DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Pessoa { Id = 2, Nome = "Maria Santos", DataNascimento = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc) }
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        // Act
        var result = await _pessoaService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_ComFiltro_DeveRetornarPessoasFiltradas()
    {
        // Arrange
        var searchTerm = "João";
        var pessoas = new List<Pessoa>
        {
            new Pessoa { Id = 1, Nome = "João Silva", DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        // Act
        var result = await _pessoaService.GetAllAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data.First().Nome.Should().Contain("João");
    }

    #endregion

    #region GetPaginateAsync Tests

    [Fact]
    public async Task GetPaginateAsync_SemFiltro_DeveRetornarPessoasPaginadas()
    {
        // Arrange
        var pessoas = new List<Pessoa>
        {
            new Pessoa { Id = 1, Nome = "João Silva", DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Pessoa { Id = 2, Nome = "Maria Santos", DataNascimento = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc) }
        };

        _pessoaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        _pessoaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                It.IsAny<TipoTransacao>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        // Act
        var result = await _pessoaService.GetPaginateAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalItems.Should().Be(2);
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorNome_DeveRetornarPessoasFiltradas()
    {
        // Arrange
        var searchTerm = "João";
        var pessoas = new List<Pessoa>
        {
            new Pessoa { Id = 1, Nome = "João Silva", DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        _pessoaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        _pessoaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                It.IsAny<TipoTransacao>(),
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        // Act
        var result = await _pessoaService.GetPaginateAsync(searchTerm: searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Nome.Should().Contain("João");
    }

    [Fact]
    public async Task GetPaginateAsync_ComTotaisCalculados_DeveRetornarPessoasComTotais()
    {
        // Arrange
        var pessoaId = 1;
        var pessoas = new List<Pessoa>
        {
            new Pessoa { Id = pessoaId, Nome = "João Silva", DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        _pessoaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        _pessoaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Receita, pessoaId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Despesa, pessoaId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        // Act
        var result = await _pessoaService.GetPaginateAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().TotalReceitas.Should().Be(1000m);
        result.Data.Items.First().TotalDespesas.Should().Be(500m);
        result.Data.Items.First().Saldo.Should().Be(500m);
    }

    [Fact]
    public async Task GetPaginateAsync_ComValoresNegativos_DeveUsarValoresPadrao()
    {
        // Arrange
        var pessoas = new List<Pessoa>();

        _pessoaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        _pessoaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _pessoaService.GetPaginateAsync(skip: -1, take: -1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        // Verifica que os valores foram normalizados (skip >= 0, take >= 1)
        _pessoaRepositoryMock.Verify(x => x.PaginateAsync(
            It.Is<int>(s => s >= 0),
            It.Is<int>(t => t >= 1),
            It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, bool>>?>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Pessoa, object>>?>(),
            It.IsAny<OrderDirection>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}

