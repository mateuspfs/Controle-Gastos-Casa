import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Table, Thead, Tbody, Tr, Th, Td } from '../Table';

describe('Table', () => {
  it('deve renderizar tabela', () => {
    const { container } = render(
      <Table>
        <tbody>
          <tr>
            <td>Conteúdo</td>
          </tr>
        </tbody>
      </Table>
    );
    const table = container.querySelector('table');
    expect(table).toBeInTheDocument();
  });

  it('deve aplicar className customizada', () => {
    const { container } = render(
      <Table className="custom-class">
        <tbody>
          <tr>
            <td>Conteúdo</td>
          </tr>
        </tbody>
      </Table>
    );
    const table = container.querySelector('table');
    expect(table?.className).toContain('custom-class');
  });
});

describe('Thead', () => {
  it('deve renderizar cabeçalho', () => {
    const { container } = render(
      <Table>
        <Thead>
          <tr>
            <th>Header</th>
          </tr>
        </Thead>
      </Table>
    );
    const thead = container.querySelector('thead');
    expect(thead).toBeInTheDocument();
  });
});

describe('Tbody', () => {
  it('deve renderizar corpo da tabela', () => {
    const { container } = render(
      <Table>
        <Tbody>
          <tr>
            <td>Conteúdo</td>
          </tr>
        </Tbody>
      </Table>
    );
    const tbody = container.querySelector('tbody');
    expect(tbody).toBeInTheDocument();
  });
});

describe('Tr', () => {
  it('deve renderizar linha da tabela', () => {
    const { container } = render(
      <Table>
        <tbody>
          <Tr>
            <td>Conteúdo</td>
          </Tr>
        </tbody>
      </Table>
    );
    const tr = container.querySelector('tr');
    expect(tr).toBeInTheDocument();
  });

  it('deve aplicar className customizada', () => {
    const { container } = render(
      <Table>
        <tbody>
          <Tr className="custom-class">
            <td>Conteúdo</td>
          </Tr>
        </tbody>
      </Table>
    );
    const tr = container.querySelector('tr');
    expect(tr?.className).toContain('custom-class');
  });
});

describe('Th', () => {
  it('deve renderizar célula de cabeçalho', () => {
    render(
      <Table>
        <Thead>
          <Tr>
            <Th>Header</Th>
          </Tr>
        </Thead>
      </Table>
    );
    expect(screen.getByText('Header')).toBeInTheDocument();
  });
});

describe('Td', () => {
  it('deve renderizar célula de dados', () => {
    render(
      <Table>
        <tbody>
          <Tr>
            <Td>Dados</Td>
          </Tr>
        </tbody>
      </Table>
    );
    expect(screen.getByText('Dados')).toBeInTheDocument();
  });
});

