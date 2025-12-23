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

public class CategoriaServiceTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
    private readonly IMapper _mapper;
    private readonly CategoriaService _categoriaService;

    public CategoriaServiceTests()
    {
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _mapper = TestHelper.CreateMapper();
        _categoriaService = new CategoriaService(
            _categoriaRepositoryMock.Object,
            _transacaoRepositoryMock.Object,
            _mapper
        );
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var categoriaDto = new CategoriaDto
        {
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var categoriaEntity = new Categoria
        {
            Id = 1,
            Descricao = categoriaDto.Descricao,
            Finalidade = categoriaDto.Finalidade
        };

        _categoriaRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .Callback<Categoria, CancellationToken>((c, ct) => { c.Id = categoriaEntity.Id; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _categoriaService.CreateAsync(categoriaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be(categoriaDto.Descricao);
        result.Data.Finalidade.Should().Be(categoriaDto.Finalidade);
        
        _categoriaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComErroNoRepository_DeveRetornarFalha()
    {
        // Arrange
        var categoriaDto = new CategoriaDto
        {
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        _categoriaRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro no banco de dados"));

        // Act
        var result = await _categoriaService.CreateAsync(categoriaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Ocorreu um erro ao criar a categoria");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarCategoria()
    {
        // Arrange
        var categoriaId = 1;
        var categoriaEntity = new Categoria
        {
            Id = categoriaId,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoriaEntity);

        // Act
        var result = await _categoriaService.GetByIdAsync(categoriaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(categoriaId);
        result.Data.Descricao.Should().Be(categoriaEntity.Descricao);
        
        _categoriaRepositoryMock.Verify(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var categoriaId = 999;

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _categoriaService.GetByIdAsync(categoriaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Categoria não encontrada");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarCategoria()
    {
        // Arrange
        var categoriaId = 1;
        var categoriaExistente = new Categoria
        {
            Id = categoriaId,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var categoriaDtoAtualizado = new CategoriaDto
        {
            Descricao = "Alimentação Atualizada",
            Finalidade = FinalidadeCategoria.Ambas
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoriaExistente);

        _categoriaRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaId, categoriaDtoAtualizado);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be(categoriaDtoAtualizado.Descricao);
        result.Data.Finalidade.Should().Be(categoriaDtoAtualizado.Finalidade);
        
        _categoriaRepositoryMock.Verify(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()), Times.Once);
        _categoriaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var categoriaId = 999;
        var categoriaDto = new CategoriaDto
        {
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaId, categoriaDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Categoria não encontrada");
        
        _categoriaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_SemFiltro_DeveRetornarTodasCategorias()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa },
            new Categoria { Id = 2, Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita }
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        // Act
        var result = await _categoriaService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllAsync_ComFiltroPorDescricao_DeveRetornarCategoriasFiltradas()
    {
        // Arrange
        var searchTerm = "Alimentação";
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa }
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        // Act
        var result = await _categoriaService.GetAllAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data.First().Descricao.Should().Contain("Alimentação");
    }

    [Fact]
    public async Task GetAllAsync_ComFiltroPorFinalidade_DeveRetornarCategoriasFiltradas()
    {
        // Arrange
        var finalidade = (int)FinalidadeCategoria.Despesa;
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa }
        };

        _categoriaRepositoryMock
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        // Act
        var result = await _categoriaService.GetAllAsync(null, finalidade);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(1);
        result.Data.First().Finalidade.Should().Be(FinalidadeCategoria.Despesa);
    }

    #endregion

    #region GetPaginateAsync Tests

    [Fact]
    public async Task GetPaginateAsync_SemFiltro_DeveRetornarCategoriasPaginadas()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa },
            new Categoria { Id = 2, Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita }
        };

        _categoriaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        _categoriaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
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
        var result = await _categoriaService.GetPaginateAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalItems.Should().Be(2);
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorDescricao_DeveRetornarCategoriasFiltradas()
    {
        // Arrange
        var searchTerm = "Alimentação";
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa }
        };

        _categoriaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        _categoriaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
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
        var result = await _categoriaService.GetPaginateAsync(searchTerm: searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Descricao.Should().Contain("Alimentação");
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorFinalidade_DeveRetornarCategoriasFiltradas()
    {
        // Arrange
        var finalidade = (int)FinalidadeCategoria.Despesa;
        var categorias = new List<Categoria>
        {
            new Categoria { Id = 1, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa }
        };

        _categoriaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        _categoriaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
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
        var result = await _categoriaService.GetPaginateAsync(finalidade: finalidade);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Finalidade.Should().Be(FinalidadeCategoria.Despesa);
    }

    [Fact]
    public async Task GetPaginateAsync_ComTotaisCalculados_DeveRetornarCategoriasComTotais()
    {
        // Arrange
        var categoriaId = 1;
        var categorias = new List<Categoria>
        {
            new Categoria { Id = categoriaId, Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa }
        };

        _categoriaRepositoryMock
            .Setup(x => x.PaginateAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        _categoriaRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Categoria, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Receita, null, categoriaId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Despesa, null, categoriaId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        // Act
        var result = await _categoriaService.GetPaginateAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().TotalReceitas.Should().Be(0m);
        result.Data.Items.First().TotalDespesas.Should().Be(500m);
        result.Data.Items.First().Saldo.Should().Be(-500m);
    }

    #endregion
}

