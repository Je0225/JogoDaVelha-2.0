using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JogoDaVelha2._0 {
  public class Status {

    public static KeyValuePair<String, Color>  aguardandoOponenteEntrar = new KeyValuePair<String, Color>("Aguardando oponente entrar no Jogo", Color.Yellow);

    public static KeyValuePair<String, Color> aguardandoJogadaOponente = new KeyValuePair<String, Color>("Aguardando jogada do oponente", Color.LightSkyBlue);

    public static KeyValuePair<String, Color> podeJogar = new KeyValuePair<String, Color>("Pode jogar", Color.Green);

    public static KeyValuePair<String, Color> oponenteDesistiu = new KeyValuePair<String, Color>("Oponente desistiu", Color.PaleVioletRed);

    public static KeyValuePair<String, Color> oponenteOnline = new KeyValuePair<String, Color>("Oponente online", Color.Coral);

    public static KeyValuePair<String, Color> voceGanhou = new KeyValuePair<String, Color>("Você ganhou!", Color.MediumSeaGreen);

    public static KeyValuePair<String, Color> vocePerdeu = new KeyValuePair<String, Color>("Você perdeu!", Color.Brown);

    public String statusJogo = "";

  }
}
