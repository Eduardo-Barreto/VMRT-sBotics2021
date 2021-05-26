import("setup/variaveis");
import("setup/utils");
import("setup/leituras");
import("setup/movimentacao");
import("seguir_linha");
import("encruzilhadas");

bool debug = false;

void Main()
{
    if (!debug)
    {
        calibrar();
    }
    ultima_correcao = millis();
    while (!debug)
    {
        verifica_calibrar();
        seguir_linha();
        verifica_curva();
    }

    while (debug)
    {
        encoder(-300, 10);
        travar();
    }
}
