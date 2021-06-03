// PISO = 69, 84, 102
import("setup/variaveis");
import("setup/utils");
import("setup/leituras");
import("setup/movimentacao");
import("piso/seguir_linha");
import("piso/encruzilhadas");
import("piso/obstaculo");

// Variável de controle para ligar/desligar o debug
bool debug = false;
bool console = true;

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
        verifica_obstaculo();
        verifica_saida();
        seguir_linha();
        verifica_calibrar();
    }

    // Loop para debug
    while (debug)
    {
        parar();
    }
}
