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

public class TransacaoServiceTests
{
    private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
    private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
    private readonly IMapper _mapper;
    private readonly TransacaoService _transacaoService;

    public TransacaoServiceTests()
    {
        _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
        _pessoaRepositoryMock = new Mock<IPessoaRepository>();
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
        _mapper = TestHelper.CreateMapper();
        _transacaoService = new TransacaoService(
            _transacaoRepositoryMock.Object,
            _pessoaRepositoryMock.Object,
            _categoriaRepositoryMock.Object,
            _mapper
        );
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()))
            .Callback<Transacao, CancellationToken>((t, ct) => { t.Id = 1; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be(transacaoDto.Descricao);
        result.Data.Valor.Should().Be(transacaoDto.Valor);
        
        _pessoaRepositoryMock.Verify(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()), Times.Once);
        _categoriaRepositoryMock.Verify(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()), Times.Once);
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComPessoaInexistente_DeveRetornarFalha()
    {
        // Arrange
        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 999,
            CategoriaId = 1
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pessoa?)null);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Pessoa não encontrada");
        
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ComCategoriaInexistente_DeveRetornarFalha()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = 999
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Categoria não encontrada");
        
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_MenorDeIdadeComReceita_DeveRetornarFalha()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = DateTime.UtcNow.AddYears(-17) // Menor de idade
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Salário",
            Finalidade = FinalidadeCategoria.Receita
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Salário",
            Valor = 2000m,
            Tipo = TipoTransacao.Receita,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Menor de idade só pode registrar despesa");
        
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_MenorDeIdadeComDespesa_DeveRetornarSucesso()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = DateTime.UtcNow.AddYears(-17) // Menor de idade
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()))
            .Callback<Transacao, CancellationToken>((t, ct) => { t.Id = 1; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_DespesaComCategoriaReceita_DeveRetornarFalha()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Salário",
            Finalidade = FinalidadeCategoria.Receita // Categoria de receita
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa, // Tentando usar categoria de receita em despesa
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Categoria de receita não pode ser usada em despesa");
        
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReceitaComCategoriaDespesa_DeveRetornarFalha()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa // Categoria de despesa
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Salário",
            Valor = 2000m,
            Tipo = TipoTransacao.Receita, // Tentando usar categoria de despesa em receita
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Categoria de despesa não pode ser usada em receita");
        
        _transacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ComCategoriaAmbas_DeveRetornarSucesso()
    {
        // Arrange
        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Outros",
            Finalidade = FinalidadeCategoria.Ambas
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Transação",
            Valor = 100m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()))
            .Callback<Transacao, CancellationToken>((t, ct) => { t.Id = 1; })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transacaoService.CreateAsync(transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ComIdExistente_DeveRetornarTransacao()
    {
        // Arrange
        var transacaoId = 1;
        var transacaoEntity = new Transacao
        {
            Id = transacaoId,
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 1,
            CategoriaId = 1
        };

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoEntity);

        // Act
        var result = await _transacaoService.GetByIdAsync(transacaoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(transacaoId);
        result.Data.Descricao.Should().Be(transacaoEntity.Descricao);
        
        _transacaoRepositoryMock.Verify(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var transacaoId = 999;

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transacao?)null);

        // Act
        var result = await _transacaoService.GetByIdAsync(transacaoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Transação não encontrada");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ComIdExistente_DeveDeletarTransacao()
    {
        // Arrange
        var transacaoId = 1;
        var transacaoEntity = new Transacao
        {
            Id = transacaoId,
            Descricao = "Compra no supermercado",
            Valor = 150.50m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 1,
            CategoriaId = 1
        };

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoEntity);

        _transacaoRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transacaoService.DeleteAsync(transacaoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        
        _transacaoRepositoryMock.Verify(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()), Times.Once);
        _transacaoRepositoryMock.Verify(x => x.DeleteAsync(transacaoEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var transacaoId = 999;

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transacao?)null);

        // Act
        var result = await _transacaoService.DeleteAsync(transacaoId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Transação não encontrada");
        
        _transacaoRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetTotaisGeraisAsync Tests

    [Fact]
    public async Task GetTotaisGeraisAsync_SemFiltros_DeveRetornarTotais()
    {
        // Arrange
        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Receita, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Despesa, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        // Act
        var result = await _transacaoService.GetTotaisGeraisAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalReceitas.Should().Be(1000m);
        result.Data.TotalDespesas.Should().Be(500m);
        result.Data.SaldoLiquido.Should().Be(500m);
    }

    [Fact]
    public async Task GetTotaisGeraisAsync_ComFiltroTipoReceita_DeveRetornarApenasReceitas()
    {
        // Arrange
        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Receita, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);

        // Act
        var result = await _transacaoService.GetTotaisGeraisAsync(tipo: TipoTransacao.Receita);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalReceitas.Should().Be(1000m);
        result.Data.TotalDespesas.Should().Be(0m);
        result.Data.SaldoLiquido.Should().Be(1000m);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveAtualizarTransacao()
    {
        // Arrange
        var transacaoId = 1;
        var transacaoExistente = new Transacao
        {
            Id = transacaoId,
            Descricao = "Compra antiga",
            Valor = 100m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 1,
            CategoriaId = 1
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Alimentação",
            Finalidade = FinalidadeCategoria.Despesa
        };

        var transacaoDtoAtualizado = new TransacaoDto
        {
            Descricao = "Compra atualizada",
            Valor = 200m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoExistente);

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        _transacaoRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transacaoService.UpdateAsync(transacaoId, transacaoDtoAtualizado);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be(transacaoDtoAtualizado.Descricao);
        result.Data.Valor.Should().Be(transacaoDtoAtualizado.Valor);
        
        _transacaoRepositoryMock.Verify(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()), Times.Once);
        _transacaoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ComIdInexistente_DeveRetornarFalha()
    {
        // Arrange
        var transacaoId = 999;
        var transacaoDto = new TransacaoDto
        {
            Descricao = "Compra",
            Valor = 100m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 1,
            CategoriaId = 1
        };

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transacao?)null);

        // Act
        var result = await _transacaoService.UpdateAsync(transacaoId, transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Transação não encontrada");
        
        _transacaoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_MenorDeIdadeTentandoAtualizarParaReceita_DeveRetornarFalha()
    {
        // Arrange
        var transacaoId = 1;
        var transacaoExistente = new Transacao
        {
            Id = transacaoId,
            Descricao = "Despesa",
            Valor = 100m,
            Tipo = TipoTransacao.Despesa,
            DataTransacao = DateTime.UtcNow,
            PessoaId = 1,
            CategoriaId = 1
        };

        var pessoa = new Pessoa
        {
            Id = 1,
            Nome = "João Silva",
            DataNascimento = DateTime.UtcNow.AddYears(-17) // Menor de idade
        };

        var categoria = new Categoria
        {
            Id = 1,
            Descricao = "Salário",
            Finalidade = FinalidadeCategoria.Receita
        };

        var transacaoDto = new TransacaoDto
        {
            Descricao = "Salário",
            Valor = 2000m,
            Tipo = TipoTransacao.Receita, // Tentando mudar para receita
            DataTransacao = DateTime.UtcNow,
            PessoaId = pessoa.Id,
            CategoriaId = categoria.Id
        };

        _transacaoRepositoryMock
            .Setup(x => x.GetByIdAsync(transacaoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacaoExistente);

        _pessoaRepositoryMock
            .Setup(x => x.GetByIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        _categoriaRepositoryMock
            .Setup(x => x.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        // Act
        var result = await _transacaoService.UpdateAsync(transacaoId, transacaoDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Menor de idade só pode registrar despesa");
        
        _transacaoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Transacao>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetPaginateAsync Tests

    [Fact]
    public async Task GetPaginateAsync_SemFiltros_DeveRetornarTransacoesPaginadas()
    {
        // Arrange
        var transacoes = new List<Transacao>
        {
            new() {
                Id = 1,
                Descricao = "Compra 1",
                Valor = 100m,
                Tipo = TipoTransacao.Despesa,
                DataTransacao = DateTime.UtcNow,
                PessoaId = 1,
                CategoriaId = 1
            },
            new() {
                Id = 2,
                Descricao = "Compra 2",
                Valor = 200m,
                Tipo = TipoTransacao.Despesa,
                DataTransacao = DateTime.UtcNow,
                PessoaId = 1,
                CategoriaId = 1
            }
        };

        _transacaoRepositoryMock
            .Setup(x => x.PaginateWithIncludesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoes);

        _transacaoRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        var result = await _transacaoService.GetPaginateAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalItems.Should().Be(2);
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorData_DeveRetornarTransacoesFiltradas()
    {
        // Arrange
        var dataInicio = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);
        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = 1,
                Descricao = "Compra em janeiro",
                Valor = 100m,
                Tipo = TipoTransacao.Despesa,
                DataTransacao = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                PessoaId = 1,
                CategoriaId = 1
            }
        };

        _transacaoRepositoryMock
            .Setup(x => x.PaginateWithIncludesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoes);

        _transacaoRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _transacaoService.GetPaginateAsync(
            dataInicio: dataInicio,
            dataFim: dataFim);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorPessoa_DeveRetornarTransacoesFiltradas()
    {
        // Arrange
        var pessoaId = 1;
        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = 1,
                Descricao = "Compra",
                Valor = 100m,
                Tipo = TipoTransacao.Despesa,
                DataTransacao = DateTime.UtcNow,
                PessoaId = pessoaId,
                CategoriaId = 1
            }
        };

        _transacaoRepositoryMock
            .Setup(x => x.PaginateWithIncludesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoes);

        _transacaoRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _transacaoService.GetPaginateAsync(pessoaId: pessoaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().PessoaId.Should().Be(pessoaId);
    }

    [Fact]
    public async Task GetPaginateAsync_ComFiltroPorTipo_DeveRetornarTransacoesFiltradas()
    {
        // Arrange
        var tipo = TipoTransacao.Receita;
        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = 1,
                Descricao = "Salário",
                Valor = 2000m,
                Tipo = tipo,
                DataTransacao = DateTime.UtcNow,
                PessoaId = 1,
                CategoriaId = 1
            }
        };

        _transacaoRepositoryMock
            .Setup(x => x.PaginateWithIncludesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoes);

        _transacaoRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _transacaoService.GetPaginateAsync(tipo: tipo);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Tipo.Should().Be(tipo);
    }

    [Fact]
    public async Task GetPaginateAsync_ComValoresNegativos_DeveUsarValoresPadrao()
    {
        // Arrange
        var transacoes = new List<Transacao>();

        _transacaoRepositoryMock
            .Setup(x => x.PaginateWithIncludesAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
                It.IsAny<OrderDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(transacoes);

        _transacaoRepositoryMock
            .Setup(x => x.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _transacaoService.GetPaginateAsync(skip: -1, take: -1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        // Verifica que os valores foram normalizados (skip >= 0, take >= 1)
        _transacaoRepositoryMock.Verify(x => x.PaginateWithIncludesAsync(
            It.Is<int>(s => s >= 0),
            It.Is<int>(t => t >= 1),
            It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, bool>>?>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Transacao, object>>?>(),
            It.IsAny<OrderDirection>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task GetTotaisGeraisAsync_ComFiltroTipoDespesa_DeveRetornarApenasDespesas()
    {
        // Arrange
        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Despesa, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500m);

        // Act
        var result = await _transacaoService.GetTotaisGeraisAsync(tipo: TipoTransacao.Despesa);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalReceitas.Should().Be(0m);
        result.Data.TotalDespesas.Should().Be(500m);
        result.Data.SaldoLiquido.Should().Be(-500m);
    }

    [Fact]
    public async Task GetTotaisGeraisAsync_ComFiltrosCombinados_DeveRetornarTotaisFiltrados()
    {
        // Arrange
        var pessoaId = 1;
        var categoriaId = 2;
        var dataInicio = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Receita, pessoaId, categoriaId, dataInicio, dataFim, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000m);

        _transacaoRepositoryMock
            .Setup(x => x.SomarTransacoesPorTipoAsync(
                TipoTransacao.Despesa, pessoaId, categoriaId, dataInicio, dataFim, It.IsAny<CancellationToken>()))
            .ReturnsAsync(300m);

        // Act
        var result = await _transacaoService.GetTotaisGeraisAsync(
            dataInicio: dataInicio,
            dataFim: dataFim,
            pessoaId: pessoaId,
            categoriaId: categoriaId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalReceitas.Should().Be(1000m);
        result.Data.TotalDespesas.Should().Be(300m);
        result.Data.SaldoLiquido.Should().Be(700m);
    }

    #endregion
}

