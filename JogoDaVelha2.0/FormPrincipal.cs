using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JogoDaVelha2._0 {
  public partial class FormPrincipal : Form {

    private String X { get; set; }

    private String O { get; set; }

    private Boolean EhX { get; set; }

    private String[,] Posicoes { get; set; }

    public FormPrincipal() {
      InitializeComponent();
      Posicoes = new String[3,3];
      lblMensagem.Visible = false;
      EhX = false;
      O = "O";
      X = "X";
    }

    private void ClicouNoBotao(Button botao) {
      botao.Text = EhX ? X : O;
      EhX = !EhX;
      if (AlguemGanhou(botao.Tag.ToString().Split(','), botao.Text)) {
        MessageBox.Show("ganhou");
      }
    }

    private Boolean AlguemGanhou(String[] posicoes, String valor) {
      Boolean ehJogoValido = false;
      String[] diagonal1 = new String[3];
      String[] diagonal2 = new String[3];

      Posicoes[Convert.ToInt32(posicoes[0]), Convert.ToInt32(posicoes[1])] = valor;
      for (int i = 0; i < 3; i++) {
        for (int j = 0; j < 3; j++) {
          String val = Posicoes[i, j];
          if (Posicoes[i, 0] == Posicoes[i, 1] && Posicoes[i, 0] == Posicoes[i, 2] && val != null) {
            ehJogoValido = true;
            break;
          }
          if (Posicoes[0, j] == Posicoes[1, j] && Posicoes[0, j] == Posicoes[2, j] && val != null) {
            ehJogoValido = true;
            break;
          }
          if (i == j) {
            diagonal1[j] = val;
          }
          
        }
      }
      if (diagonal1.Distinct().Count() == 1) {
        ehJogoValido = true;
      }
      //if (diagonal2.Distinct().Count() == 1) {
        //ehJogoValido = true;
     //}
      return ehJogoValido;
    }

    private void botao_Click(object sender, EventArgs e) {
      ClicouNoBotao((Button)sender);
    }

    private void Recomecar() {
      foreach (Button botao in Controls.OfType<Button>()) {
        botao.Text = "";
      }
    }

    private void btnRecomecar_Click(object sender, EventArgs e) {
      Recomecar();
    }
  }
}
