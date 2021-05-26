import("setup/variaveis");
import("setup/utils");
import("setup/leituras");
import("setup/movimentacao");
import("piso/seguir_linha");
import("piso/encruzilhadas");

// Variável de controle para ligar/desligar o debug
bool debug = true;

// Método principal
void Main()
{
    if (!debug)
    {
        calibrar();
        ultima_correcao = millis();
    }
    // Loop principal do programa
    while (!debug)
    {
        verifica_calibrar();
        seguir_linha();
        verifica_curva();
    }

    // Loop para debug
    while (debug)
    {
        print(1, "Debug!!");
    }
}
