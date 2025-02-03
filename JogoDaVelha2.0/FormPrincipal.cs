using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;

namespace JogoDaVelha2._0 {
  public partial class FormPrincipal : Form {

    private String X { get; set; }

    private String O { get; set; }

    private String MeuJogo { get; set; }

    private String JogoOponente { get; set; }

    private String PathMeuArquivo { get; set; }

    private String PathArquivoOponente {get; set; }

    private Boolean EhMinhaVez { get; set; }

    private String[,] Posicoes { get; set; }

    private String[] diagonal1 {get; set; }

    private String[] diagonal2 { get; set; }

    private Timer timer { get; set; }

    private Status Status { get; set; }

    private Int32 QuantidadeJogadasDoOponente { get; set; }

    public FormPrincipal(String meuJogo) {
      InitializeComponent();
      MeuJogo = meuJogo;
      Text += $@" - {MeuJogo}";
      Posicoes = new String[3,3];
      O = "O";
      X = "X";
      JogoOponente = MeuJogo == X ? O : X;
      EhMinhaVez = MeuJogo == X;
      Status = new Status();
      QuantidadeJogadasDoOponente = 0;
      timer = new Timer();
      timer.Tick += new EventHandler(ChecaStatus);
      timer.Start();
      PathArquivoOponente = AppDomain.CurrentDomain.BaseDirectory + JogoOponente;
      PathMeuArquivo = AppDomain.CurrentDomain.BaseDirectory + MeuJogo;
    }

    private void ChecaStatus(Object sender, EventArgs e) {
      timer.Stop();
      if (!File.Exists(PathArquivoOponente)) {
        AtualizaStatus(Status.aguardandoOponenteEntrar);
      } 
      else if (File.Exists(PathArquivoOponente) && EhMinhaVez) {
        AtualizaStatus(Status.podeJogar);
      }
      else if (File.ReadAllLines(PathArquivoOponente).Length > QuantidadeJogadasDoOponente) {
        AtualizaStatus(Status.podeJogar);
        EhMinhaVez = true;
        QuantidadeJogadasDoOponente++;
        LeJogadasOponente();
      } 
      else if (File.Exists(PathArquivoOponente) && !EhMinhaVez) {
        AtualizaStatus(Status.aguardandoJogadaOponente);
      }
      else if (QuantidadeJogadasDoOponente > 0 && !File.Exists(PathArquivoOponente)) {
        AtualizaStatus(Status.oponenteDesistiu);
      }
      String ganhador = AlguemGanhou();
      if (!String.IsNullOrEmpty(ganhador)) {
        if (ganhador == MeuJogo) {
          AtualizaStatus(Status.voceGanhou);
        } else {
          AtualizaStatus(Status.vocePerdeu);
        }
        VarreBotoesDoJogo(false);
      }
      timer.Start();
    }

    private void AtualizaStatus(KeyValuePair<String, Color> status) {
      Status.statusJogo = status.Key;
      lblStatus.BackColor = status.Value;
      lblStatus.Text = status.Key;
    }

    private void ClicouNoBotao(Button botao) {
      if (!EhMinhaVez) {
        lblStatus.Text = Status.aguardandoJogadaOponente.Key;
        lblStatus.BackColor = Status.aguardandoJogadaOponente.Value;
        Status.statusJogo = Status.aguardandoJogadaOponente.Key;
        VarreBotoesDoJogo(false);
      } else {
        lblStatus.Text = Status.podeJogar.Key;
        lblStatus.BackColor = Status.podeJogar.Value;
        Status.statusJogo = Status.podeJogar.Key;

        VarreBotoesDoJogo(true);
        botao.Text = MeuJogo;
        botao.Enabled = false;
        RegistraMinhasJogadas(botao);
      }
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
      File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + MeuJogo, minhasJogadas);
      Posicoes[Convert.ToInt32(posicao[0]), Convert.ToInt32(posicao[1])] = MeuJogo;
    }

    private String AlguemGanhou() {
      String quemGanhou = ""; 
      diagonal1 = new String[3];
      diagonal2 = new String[3];
      
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
      return quemGanhou;
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
    }

    private void btnRecomecar_Click(object sender, EventArgs e) {
      RecomecarJogo();
    }

    private void botao_Click(object sender, EventArgs e) {
      ClicouNoBotao((Button)sender);
    }
  }
}
