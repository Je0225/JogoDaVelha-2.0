using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;

namespace JogoDaVelha2._0 {
  public partial class FormPrincipal : Form {

    private String MeuJogo { get; set; }

    private String JogoOponente { get; set; }

    private String PathMeuArquivo { get; set; }

    private String PathArquivoOponente {get; set; } 

    private Boolean EhMinhaVez { get; set; }

    private Boolean OponenteJaEsteveOnline { get; set; }

    private String[,] Posicoes { get; set; }

    private Timer Timer { get; set; }

    private Int32 QuantidadeJogadasDoOponente { get; set; }

    private Int32 Vitorias { get; set; }

    private Int32 Derrotas { get; set; }

    private Int32 Empates { get; set; }

    private String[] diagonal1 { get; set; }

    private String[] diagonal2 { get; set; }

    public FormPrincipal(String meuJogo) {
      InitializeComponent();
      MeuJogo = meuJogo;
      JogoOponente = MeuJogo == "X" ? "O" : "X";
      PathArquivoOponente = AppDomain.CurrentDomain.BaseDirectory + JogoOponente;
      PathMeuArquivo = AppDomain.CurrentDomain.BaseDirectory + MeuJogo;

      EhMinhaVez = !File.Exists(PathArquivoOponente);
      OponenteJaEsteveOnline = File.Exists(PathArquivoOponente);
      Posicoes = new String[3,3];

      QuantidadeJogadasDoOponente = 0;
      Vitorias = 0; 
      Derrotas = 0;
      Empates = 0;

      Timer = new Timer();
      Timer.Interval = 1000;
      Timer.Tick += new EventHandler(ChecaStatus);
      Timer.Start();

      Text += $@" - {MeuJogo}";
      btnRecomecar.Enabled = false;
    }

    private Boolean OponenteDesistiu() {
      return (OponenteJaEsteveOnline) && !File.Exists(PathArquivoOponente);
    }

    private Boolean OponenteNuncaEsteveOnline() {
      return !File.Exists(PathArquivoOponente) && !OponenteJaEsteveOnline;
    }

    private Boolean OponenteJogou() {
      return File.ReadAllLines(PathArquivoOponente).Length > QuantidadeJogadasDoOponente && !Status.statusJogo.Equals(Status.aguardandoOponenteEntrar);
    }
    
    private Boolean PodeJogar() {
      return File.Exists(PathArquivoOponente) && EhMinhaVez && QuantidadeJogadasDoOponente == 0;
    }

    private Boolean EhVezDoOponente() {
      return File.Exists(PathArquivoOponente) && !EhMinhaVez && (!Status.statusJogo.Equals(Status.aguardandoOponenteEntrar) && !Status.statusJogo.Equals(Status.vocePerdeu) && !Status.statusJogo.Equals(Status.voceGanhou) && !Status.statusJogo.Equals(Status.deuEmpate));
    }

    private Boolean OponenteRecomecouJogo() {
      return File.ReadAllLines(PathArquivoOponente).Length == 0 && QuantidadeJogadasDoOponente > 0 && (Status.statusJogo.Equals(Status.aguardandoOponenteEntrar));
    }

    private Boolean NaoPodeRecomecar() {
      return Status.statusJogo.Equals(Status.vocePerdeu) || Status.statusJogo.Equals(Status.voceGanhou) || Status.statusJogo.Equals(Status.deuEmpate);
    }

    private void ChecaStatus(Object sender, EventArgs e) {
      Timer.Stop();
      if (OponenteDesistiu()) {
        AtualizaStatus(Status.oponenteDesistiu);
        RecomecarJogo();
        EhMinhaVez = true;
        QuantidadeJogadasDoOponente = 0;
      }
      else if (OponenteNuncaEsteveOnline()) {
        AtualizaStatus(Status.aguardandoOponenteEntrar);
      } 
      else if (OponenteRecomecouJogo()) {
        QuantidadeJogadasDoOponente = 0;
        AtualizaStatus(EhMinhaVez ? Status.podeJogar : Status.aguardandoJogadaOponente);
      }
      else if (OponenteJogou()){
        AtualizaStatus(Status.podeJogar);
        EhMinhaVez = true;
        QuantidadeJogadasDoOponente++;
        LeJogadasOponente();
      } 
      else if (PodeJogar()) {
        AtualizaStatus(Status.podeJogar);
        OponenteJaEsteveOnline = true;
      }
      else if (EhVezDoOponente()) {
        AtualizaStatus(Status.aguardandoJogadaOponente);
        OponenteJaEsteveOnline = true;
      }
      btnRecomecar.Enabled = NaoPodeRecomecar();
      String ganhador = AlguemGanhou2();
      if (!String.IsNullOrEmpty(ganhador)) {
        if (ganhador == MeuJogo) {
          if (!Status.statusJogo.Equals(Status.voceGanhou)) {
            Vitorias++;
          }
          AtualizaStatus(Status.voceGanhou);
        } else if (ganhador == JogoOponente) {
          if (!Status.statusJogo.Equals(Status.vocePerdeu)) {
            Derrotas++;
          }
          AtualizaStatus(Status.vocePerdeu);
        } else if (ganhador == "empate") {
          if (!Status.statusJogo.Equals(Status.deuEmpate)) {
            Empates++;
          }
          AtualizaStatus(Status.deuEmpate);
        }
        btnRecomecar.Enabled = true;
        VarreBotoesDoJogo(false);
        AtualizaLabelsPlacar();
      }
      Timer.Start();
    }

    private void AtualizaLabelsPlacar() {
      lblVitoriasRes.Text = Vitorias.ToString();
      lblDerrotasRes.Text = Derrotas.ToString();
      lblEmpatesRes.Text = Empates.ToString();
    }

    private void AtualizaStatus(KeyValuePair<String, Color> status) {
      Status.statusJogo = status;
      lblStatus.BackColor = status.Value;
      lblStatus.Text = status.Key;
    }

    private void ClicouNoBotao(Button botao) {
      if (!EhMinhaVez || !Status.statusJogo.Equals(Status.podeJogar)) {
        return;
      }
      VarreBotoesDoJogo(true);
      botao.Text = MeuJogo;
      botao.Enabled = false;
      RegistraMinhasJogadas(botao);
      EhMinhaVez = !EhMinhaVez;
    }

    private void LeJogadasOponente() {
      String[] jogadasOponente = File.ReadAllLines(PathArquivoOponente);
      if (jogadasOponente.Length == 0) {
        return;
      }
      String[] indice = jogadasOponente.Last().Split(',');
      Posicoes[Convert.ToInt32(indice[0]), Convert.ToInt32(indice[1])] = JogoOponente;
      foreach (Button botao in pnlBotoesJogo.Controls) {
        String[] posicao = botao.Tag.ToString().Split(',');
        botao.Text = Posicoes[Convert.ToInt32(posicao[0]), Convert.ToInt32(posicao[1])];
      } 
    }

    private void RegistraMinhasJogadas(Button botao) { 
      String[] posicao = botao.Tag.ToString().Split(',');
      String minhasJogadas = File.ReadAllText(PathMeuArquivo);
      minhasJogadas += $"{Convert.ToInt32(posicao[0])},{Convert.ToInt32(posicao[1])}\n";
      File.WriteAllText(PathMeuArquivo, minhasJogadas);
      Posicoes[Convert.ToInt32(posicao[0]), Convert.ToInt32(posicao[1])] = MeuJogo;
    }

    private String AlguemGanhou2() {
      String res = "";
      String valoresColunas = "";
      String valoresLinhas = ""; 
      String valoresDiagonal = $"{Posicoes[0, 0] ?? "N"}{Posicoes[1, 1] ?? "N"}{Posicoes[2, 2] ?? "N"}";
      String valoresDiagonal2 = $"{Posicoes[0, 2] ?? "N"}{Posicoes[1, 1] ?? "N"}{Posicoes[2, 0] ?? "N"}";

      for (int i = 0; i < 3; i++) {
        valoresLinhas += $"{Posicoes[0, i] ?? "N"}{Posicoes[1, i] ?? "N"}{Posicoes[2, i] ?? "N"},";
        valoresColunas += $"{Posicoes[i, 0] ?? "N"}{Posicoes[i, 1] ?? "N"}{Posicoes[i, 2] ?? "N"},";
      }
      if (!valoresColunas.Contains('N') && valoresColunas.Distinct().Count() == 3 &&
          !valoresLinhas.Contains('N') && valoresLinhas.Distinct().Count() == 3) {
        return "empate";
      }
      res = QuemGanhou(valoresDiagonal.Split(','));
      res = res == "" ? QuemGanhou(valoresDiagonal2.Split(',')) : res;
      res = res == "" ? QuemGanhou(valoresLinhas.Split(',')) : res;
      return res == "" ? QuemGanhou(valoresColunas.Split(',')) : res;
    }

    private String QuemGanhou(String[] valores) {
      foreach (String valor in valores) {
        if (valor.Distinct().Count() == 1 && !valor.Contains('N')) {
          return valor.Distinct().First().ToString();
        }
      }
      return "";
    }

    private String AlguemGanhou() {
      String quemGanhou = ""; 
      diagonal1 = new String[3];
      diagonal2 = new String[3];
      Boolean deuEmpate = false;

      foreach (String posicao in Posicoes) {
        if (posicao == null) {
          deuEmpate = false;
          break;
        }
        deuEmpate = true;
      }
      for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
          String val = Posicoes[i, j];
          if (Posicoes[i, 0] == Posicoes[i, 1] && Posicoes[i, 0] == Posicoes[i, 2] && val != null) {
            quemGanhou = val;
            break;
          }
          if (Posicoes[0, j] == Posicoes[1, j] && Posicoes[0, j] == Posicoes[2, j] && val != null) {
            quemGanhou = val;
            break;
          }
          if (i == j) {
            diagonal1[j] = val;
          }
          if ((i == 0 && j == 2) || (i == 1 && j == 1) || (i == 2 && j == 0)) {
            diagonal2[j] = val;
          }
        }
      }
      if (diagonal1.Distinct().Count() == 1 && diagonal1[0] != null && diagonal1[1] != null && diagonal1[2] != null) {
        quemGanhou = diagonal1.Distinct().First();
      }
      if (diagonal2.Distinct().Count() == 1 && diagonal2[0] != null && diagonal2[1] != null && diagonal2[2] != null) {
        quemGanhou = diagonal2.Distinct().First();
      }
      return deuEmpate ? "empate" : quemGanhou;
    }

    private void VarreBotoesDoJogo(Boolean habilitar, String text = null) {
      foreach (Button botao in pnlBotoesJogo.Controls) {
        botao.Text = text ?? botao.Text;
        botao.Enabled = habilitar;
      }
    }

    private void RecomecarJogo() {
      VarreBotoesDoJogo(true, "");
      for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
          Posicoes[i, j] = null;
        }
      }
      File.WriteAllText(PathMeuArquivo, "");
      if (!Status.statusJogo.Equals(Status.oponenteDesistiu)) {
        AtualizaStatus(Status.aguardandoOponenteEntrar);
      }
    }

    private void btnRecomecar_Click(object sender, EventArgs e) {
      RecomecarJogo();
    }

    private void botao_Click(object sender, EventArgs e) {
      ClicouNoBotao((Button)sender);
    }
  }
}
