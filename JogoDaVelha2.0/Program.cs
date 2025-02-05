using System;
using System.IO;
using System.Windows.Forms;

namespace JogoDaVelha2._0 {
  internal static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      String meuJogo;
      if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "X") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "O")) {
        MessageBox.Show(@"O jogo não será aberto pois já existem dois jogadores.");
        return;
      }
      if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "X")) {
        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "O","");
        meuJogo = "O";
      } else {
        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "X", "");
        meuJogo = "X";
      }
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      FormPrincipal form = new FormPrincipal(meuJogo);
      Application.Run(form);
      File.Delete(AppDomain.CurrentDomain.BaseDirectory + meuJogo);
    }
  }
}
