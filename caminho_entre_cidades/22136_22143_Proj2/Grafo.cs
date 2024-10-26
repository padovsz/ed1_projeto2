using System;
using System.Collections.Generic;
using System.Windows.Forms;

// Nome: Hugo Gomes Soares - RA: 22136
// Nome: Maria Eduarda de Jesus Padovan - RA: 22143
namespace apGrafo
{
    class Grafo
    {
        private const int NUM_VERTICES = 100;

        Vertice[] vertices;

        Ligacao[,] adjMatrix;  

        int numVerts;      // tamanho lógico (número de vértices atual)

        DataGridView dgv;  //para exibição da matriz

        // Variáveis para Djikstra
        DistOriginal[] percurso;
        int infinity = 10000000;
        int verticeAtual;     // global usada para indicar o vértice atualmente sendo visitado
        int doInicioAteAtualDist; // global usada para ajustar menor caminho com Dijkstra
        int doInicioAteAtualTemp;

        public Grafo(DataGridView dgv)
        {
            this.dgv = dgv;
            vertices = new Vertice[NUM_VERTICES];
            adjMatrix = new Ligacao[NUM_VERTICES, NUM_VERTICES];
            numVerts = 0;

            for (int j = 0; j < NUM_VERTICES; j++) // inicia toda a matriz
                for (int k = 0; k < NUM_VERTICES; k++)
                    adjMatrix[j, k] = new Ligacao("", "", infinity, infinity);       // distância tão grande que não existe

            percurso = new DistOriginal[NUM_VERTICES];
        }

        public void NovoVertice(string label)
        {
            vertices[numVerts] = new Vertice(label);
            numVerts++;
            if (dgv != null) // se foi passado como parâmetro um dataGridView para exibição
            { // se realiza o seu ajuste para a quantidade de vértices
                dgv.RowCount = numVerts + 1;
                dgv.ColumnCount = numVerts + 1;
                dgv.Columns[numVerts].Width = 45;
            }
        }

        public void EditarVertice(int v, string novaLabel)
        {
            if (v < numVerts)
                vertices[v].Rotulo = novaLabel;
        }

        public void NovaAresta(int inicio, int fim)
        {
            adjMatrix[inicio, fim] = new Ligacao(vertices[inicio].Rotulo, vertices[fim].Rotulo, 1, 0);
            // adjMatrix[fim, inicio] = 1; ISSO GERA CICLOS!!!
        }

        public void NovaAresta(int inicio, int fim, Ligacao peso)
        {
            adjMatrix[inicio, fim] = peso;
        }

        public void EditarAresta(int inicio, int fimOrig, int novoFim, Ligacao novoPeso)
        {
            adjMatrix[inicio, fimOrig] = new Ligacao("", "", infinity, infinity);
            adjMatrix[inicio, novoFim] = novoPeso;
        }

        public void ExibirVertice(int v)
        {
            Console.Write(vertices[v].Rotulo + " ");
        }
        public void ExibirVertice(int v, TextBox txt)
        {
            txt.Text += vertices[v].Rotulo + " ";
        }

        public int SemSucessores() // encontra e retorna a linha de um vértice sem sucessores
        {
            bool temAresta;
            for (int linha = 0; linha < numVerts; linha++)
            {
                temAresta = false;
                for (int col = 0; col < numVerts; col++)
                    if (adjMatrix[linha, col].Distancia > 0)
                    {
                        temAresta = true;
                        break;
                    }
                if (!temAresta)
                    return linha;
            }
            return -1;  // não achou linha sem sucessor
        }

        public void RemoverVertice(int vert)
        {
            if (dgv != null)
            {
                MessageBox.Show($"Matriz de Adjacências antes de remover vértice {vert}");
                ExibirAdjacencias();
            }
            if (vert != numVerts - 1)
            {
                for (int j = vert; j < numVerts - 1; j++) // remove vértice do vetor
                    vertices[j] = vertices[j + 1];
                // remove vértice da matriz
                for (int row = vert; row < numVerts; row++)
                    MoverLinhas(row, numVerts - 1);
                for (int col = vert; col < numVerts; col++)
                    MoverColunas(col, numVerts - 1);
            }
            numVerts--;
            if (dgv != null)
            {
                MessageBox.Show($"Matriz de Adjacências após remover vértice {vert}");
                ExibirAdjacencias();
                MessageBox.Show("Retornando à ordenação");
            }
        }

        public void RemoverAresta(int inicio, int fim)
        {
            adjMatrix[inicio, fim] = new Ligacao("", "", infinity, infinity);
        }

        private void MoverLinhas(int row, int length)
        {
            if (row != numVerts - 1)
                for (int col = 0; col < length; col++)
                    adjMatrix[row, col] = adjMatrix[row + 1, col]; // desloca para excluir
        }
        private void MoverColunas(int col, int length)
        {
            if (col != numVerts - 1)
                for (int row = 0; row < length; row++)
                    adjMatrix[row, col] = adjMatrix[row, col + 1]; // desloca para excluir
        }

        public void ExibirAdjacencias()
        {
            dgv.RowCount = numVerts + 1;
            dgv.ColumnCount = numVerts + 1;
            for (int j = 0; j < numVerts; j++)
            {
                dgv.Rows[j + 1].Cells[0].Value = vertices[j].Rotulo;
                dgv.Rows[0].Cells[j + 1].Value = vertices[j].Rotulo;
                for (int k = 0; k < numVerts; k++)
                    dgv.Rows[j + 1].Cells[k + 1].Value = Convert.ToString(adjMatrix[j, k]);
            }
        }

        public String OrdenacaoTopologica()
        {
            Stack<String> gPilha = new Stack<String>(); //guarda a sequência de vértices
            int origVerts = numVerts;
            while (numVerts > 0)
            {
                int currVertex = SemSucessores();
                if (currVertex == -1)
                    return "Erro: grafo possui ciclos.";
                gPilha.Push(vertices[currVertex].Rotulo); // empilha vértice
                RemoverVertice(currVertex);
            }
            String resultado = "Sequência da Ordenação Topológica: ";
            while (gPilha.Count > 0)
                resultado += gPilha.Pop() + " "; // desempilha para exibir
            return resultado;
        }

        private int ObterVerticeAdjacenteNaoVisitado(int v)
        {
            for (int j = 0; j <= numVerts - 1; j++)
                if ((adjMatrix[v, j].Distancia == 1) && (!vertices[j].FoiVisitado))
                    return j;
            return -1;
        }

        public void PercursoEmProfundidade(TextBox txt)
        {
            txt.Clear();
            Stack<int> gPilha = new Stack<int>(); // para guardar a sequência de vértices
            vertices[0].FoiVisitado = true;
            ExibirVertice(0, txt);
            gPilha.Push(0);
            int v;
            while (gPilha.Count > 0)
            {
                v = ObterVerticeAdjacenteNaoVisitado(gPilha.Peek());
                if (v == -1)
                    gPilha.Pop();
                else
                {
                    vertices[v].FoiVisitado = true;
                    ExibirVertice(v, txt);
                    gPilha.Push(v);
                }
            }
            for (int j = 0; j <= numVerts - 1; j++)
                vertices[j].FoiVisitado = false;
        }

        public void percursoPorLargura(TextBox txt)
        {
            txt.Clear();
            Queue<int> gQueue = new Queue<int>();
            vertices[0].FoiVisitado = true;
            ExibirVertice(0, txt);
            gQueue.Enqueue(0);
            int vert1, vert2;
            while (gQueue.Count > 0)
            {
                vert1 = gQueue.Dequeue();
                vert2 = ObterVerticeAdjacenteNaoVisitado(vert1);
                while (vert2 != -1)
                {
                    vertices[vert2].FoiVisitado = true;
                    ExibirVertice(vert2, txt);
                    gQueue.Enqueue(vert2);
                    vert2 = ObterVerticeAdjacenteNaoVisitado(vert1);
                }
            }
            for (int i = 0; i < numVerts; i++)
                vertices[i].FoiVisitado = false;
        }

        public string Caminho(int inicioDoPercurso, int finalDoPercurso, ListBox lista, NumericUpDown distancia, NumericUpDown tempo)
        {
            for (int j = 0; j < numVerts; j++)
                vertices[j].FoiVisitado = false;

            vertices[inicioDoPercurso].FoiVisitado = true;
            for (int j = 0; j < numVerts; j++)
            {
                // anotamos no vetor percurso a distância entre o inicioDoPercurso e cada vértice
                // se não há ligação direta, o valor da distância será infinity
                int dist = adjMatrix[inicioDoPercurso, j].Distancia;
                int temp = adjMatrix[inicioDoPercurso, j].Tempo;
                percurso[j] = new DistOriginal(inicioDoPercurso, dist, temp);
            }

            for (int nTree = 0; nTree < numVerts; nTree++)
            {
                // Procuramos a saída não visitada do vértice inicioDoPercurso com a menor distância
                int indiceDoMenor = ObterMenor();

                // e anotamos essa menor distância
                int distanciaMinima = percurso[indiceDoMenor].distancia;

                // o vértice com a menor distância passa a ser o vértice atual
                // para compararmos com a distância calculada em AjustarMenorCaminho()
                verticeAtual = indiceDoMenor;
                doInicioAteAtualDist = percurso[indiceDoMenor].distancia;
                doInicioAteAtualTemp = percurso[indiceDoMenor].tempo;

                // visitamos o vértice com a menor distância desde o inicioDoPercurso
                vertices[verticeAtual].FoiVisitado = true;
                AjustarMenorCaminho(lista);
            }
            return ExibirPercursos(inicioDoPercurso, finalDoPercurso, lista, distancia, tempo);
        }

        public int ObterMenor()
        {
            int distanciaMinima = infinity;
            int indiceDaMinima = 0;
            for (int j = 0; j < numVerts; j++)
                if (!(vertices[j].FoiVisitado) && (percurso[j].distancia < distanciaMinima))
                {
                    distanciaMinima = percurso[j].distancia;
                    indiceDaMinima = j;
                }

            return indiceDaMinima;
        }

        public void AjustarMenorCaminho(ListBox lista)
        {
            for (int coluna = 0; coluna < numVerts; coluna++)
                if (!vertices[coluna].FoiVisitado) // para cada vértice ainda não visitado
                {
                    // acessamos a distância desde o vértice atual (pode ser infinity)
                    int atualAteMargemDist = adjMatrix[verticeAtual, coluna].Distancia;
                    int atualAteMargemTemp = adjMatrix[verticeAtual, coluna].Tempo;

                    // calculamos a distância desde inicioDoPercurso passando por vertice atual até
                    // esta saída
                    int doInicioAteMargemDist = doInicioAteAtualDist + atualAteMargemDist;
                    int doInicioAteMargemTemp = doInicioAteAtualTemp + atualAteMargemTemp;

                    // quando encontra uma distância menor, marca o vértice a partir do
                    // qual chegamos no vértice de índice coluna, e a soma da distância
                    // percorrida para nele chegar
                    int distanciaDoCaminho = percurso[coluna].distancia;
                    if (doInicioAteMargemDist < distanciaDoCaminho)
                    {
                        percurso[coluna].verticePai = verticeAtual;
                        percurso[coluna].distancia = doInicioAteMargemDist;
                        percurso[coluna].tempo = doInicioAteMargemTemp;
                        //ExibirTabela(lista);
                    }
                }
        }

        public void ExibirTabela(ListBox lista)
        {
            string dist = "";
            lista.Items.Add("Vértice\tVisitado?\tPeso\tVindo de");
            for (int i = 0; i < numVerts; i++)
            {
                if (percurso[i].distancia == infinity)
                    dist = "inf";
                else
                    dist = Convert.ToString(percurso[i].distancia);
                lista.Items.Add(vertices[i].Rotulo + "\t" + vertices[i].FoiVisitado +
                "\t\t" + dist + "\t" + vertices[percurso[i].verticePai].Rotulo);
            }
            lista.Items.Add("-----------------------------------------------------");
        }

        public string ExibirPercursos(int inicioDoPercurso, int finalDoPercurso,
        ListBox lista, NumericUpDown dist, NumericUpDown temp)
        {
            /*string resultado = "";
            for (int j = 0; j < numVerts; j++)
            {
                resultado += vertices[j].Rotulo + "=";
                if (percurso[j].distancia == infinity)
                    resultado += "inf";
                else
                    resultado += percurso[j].distancia + " ";
                string pai = vertices[percurso[j].verticePai].Rotulo;
                resultado += "(" + pai + ") ";
            }

            lista.Items.Add(resultado);
            lista.Items.Add(" ");
            lista.Items.Add(" ");
            lista.Items.Add("Caminho entre " + vertices[inicioDoPercurso].Rotulo +
            " e " + vertices[finalDoPercurso].Rotulo);
            lista.Items.Add(" ");*/

            int onde = finalDoPercurso;
            Stack<string> pilha = new Stack<string>();
            if (percurso[onde].distancia != infinity)
            {
                dist.Value = percurso[onde].distancia;
                temp.Value = percurso[onde].tempo;
            }

            int cont = 0;
            while (onde != inicioDoPercurso)
            {
                onde = percurso[onde].verticePai;
                pilha.Push(vertices[onde].Rotulo);
                cont++;
            }
            
            string resultado = "";
            while (pilha.Count != 0)
            {
                lista.Items.Add(pilha.Pop());
            }
            if ((cont == 1) && (percurso[finalDoPercurso].distancia == infinity))
                resultado = "Não há caminho";
            else
                lista.Items.Add(vertices[finalDoPercurso].Rotulo);

            return resultado;
        }
    }
}
