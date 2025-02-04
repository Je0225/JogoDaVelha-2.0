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

    private Int32 QuantidadeJogadasDoOponente { get; set; }

    private Boolean OponenteJaEsteveOnline {get; set; }

    private Int32 MinhaPontuacao { get; set; }

    private Int32 PontuacaoOponente { get; set; }

    public FormPrincipal(String meuJogo) {
      InitializeComponent();
      O = "O";
      X = "X";
      MeuJogo = meuJogo;
      JogoOponente = MeuJogo == X ? O : X;
      PathArquivoOponente = AppDomain.CurrentDomain.BaseDirectory + JogoOponente;
      EhMinhaVez = !File.Exists(PathArquivoOponente);
      OponenteJaEsteveOnline = File.Exists(PathArquivoOponente);
      Text += $@" - {MeuJogo}";
      Posicoes = new String[3,3];
      QuantidadeJogadasDoOponente = 0;
      timer = new Timer();
      timer.Interval = 1000;
      timer.Tick += new EventHandler(ChecaStatus);
      timer.Start();
      PathMeuArquivo = AppDomain.CurrentDomain.BaseDirectory + MeuJogo;
      btnRecomecar.Enabled = false;
      MinhaPontuacao = 0;
      PontuacaoOponente = 0;
    }

    private Boolean OponenteDesistiu() {
      return (OponenteJaEsteveOnline) && !File.Exists(PathArquivoOponente);
    }

    private Boolean OponenteNuncaEsteveOnline() {
      return !File.Exists(PathArquivoOponente) && !OponenteJaEsteveOnline;
    }

    private Boolean OponenteJogou() {
      return File.ReadAllLines(PathArquivoOponente).Length > QuantidadeJogadasDoOponente && Status.statusJogo != Status.aguardandoOponenteEntrar.Key;
    }
    
    private Boolean PodeJogar() {
      return File.Exists(PathArquivoOponente) && EhMinhaVez && QuantidadeJogadasDoOponente == 0;
    }

    private Boolean EhVezDoOponente() {
      return File.Exists(PathArquivoOponente) && !EhMinhaVez && (Status.statusJogo != Status.aguardandoOponenteEntrar.Key && Status.statusJogo != Status.vocePerdeu.Key && Status.statusJogo != Status.voceGanhou.Key);
    }

    private Boolean OponenteRecomecouJogo() {
      return File.ReadAllLines(PathArquivoOponente).Length == 0 && QuantidadeJogadasDoOponente > 0 && (Status.statusJogo == Status.aguardandoOponenteEntrar.Key);
    }

    private void ChecaStatus(Object sender, EventArgs e) {
      timer.Stop();
      if (OponenteDesistiu()) {
        AtualizaStatus(Status.oponenteDesistiu);
        RecomecarJogo();
        EhMinhaVez = true;
        QuantidadeJogadasDoOponente = 0;
        PontuacaoOponente = 0;
        MinhaPontuacao = 0;
      }
      else if (OponenteNuncaEsteveOnline()) {
        AtualizaStatus(Status.aguardandoOponenteEntrar);
      } 
      else if (OponenteRecomecouJogo()) {
        QuantidadeJogadasDoOponente = 0;
        AtualizaStatus(EhMinhaVez ? Status.podeJogar : Status.aguardandoJogadaOponente);
        AtualizaLabelsPlacar();
      }
      else if (OponenteJogou()){
        AtualizaStatus(Status.podeJogar);
        EhMinhaVez = true;
        QuantidadeJogadasDoOponente++;
        LeJogadasOponente();
        AtualizaLabelsPlacar();
      } 
      else if (PodeJogar()) {
        AtualizaStatus(Status.podeJogar);
        OponenteJaEsteveOnline = true;
        AtualizaLabelsPlacar();
      }
      else if (EhVezDoOponente()) {
        AtualizaStatus(Status.aguardandoJogadaOponente);
        OponenteJaEsteveOnline = true;
        AtualizaLabelsPlacar();
      }
      String ganhador = AlguemGanhou();
      if (!String.IsNullOrEmpty(ganhador)) {
        if (ganhador == MeuJogo) {
          if (Status.statusJogo != Status.voceGanhou.Key) {
            MinhaPontuacao++;
          }
          AtualizaStatus(Status.voceGanhou);
        } else {
          if (Status.statusJogo != Status.vocePerdeu.Key) {
            PontuacaoOponente++;
          }
          AtualizaStatus(Status.vocePerdeu);
        }
        VarreBotoesDoJogo(false);
        btnRecomecar.Enabled = true;
        AtualizaLabelsPlacar();
      }
      timer.Start();
    }

    private void AtualizaLabelsPlacar() {
      lblPlacarX.Text = MeuJogo == X ? MinhaPontuacao.ToString() : PontuacaoOponente.ToString();
      lblPlacarO.Text = MeuJogo == O ? MinhaPontuacao.ToString() : PontuacaoOponente.ToString();
    }

    private void AtualizaStatus(KeyValuePair<String, Color> status) {
      Status.statusJogo = status.Key;
      lblStatus.BackColor = status.Value;
      lblStatus.Text = status.Key;
    }

    private void ClicouNoBotao(Button botao) {
      if (!EhMinhaVez || Status.statusJogo != Status.podeJogar.Key) {
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
      File.WriteAllText(PathMeuArquivo, "");
      if (Status.statusJogo != Status.oponenteDesistiu.Key) {
        AtualizaStatus(Status.aguardandoOponenteEntrar);
        btnRecomecar.Enabled = false;
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
