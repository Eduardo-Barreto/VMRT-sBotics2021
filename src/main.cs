import("setup/variaveis.cs");
import("setup/utils.cs");
import("setup/leituras.cs");
import("setup/movimentacao.cs");
import("piso/seguir_linha.cs");
import("piso/encruzilhadas.cs");
import("piso/obstaculo.cs");
import("piso/elevados.cs");
import("resgate/setup/movimentacao.cs");
import("resgate/mover_para.cs");
import("resgate/varredura.cs");


// Variáveis de controle para ligar/desligar o debug e console
bool debug = false;
bool console = true;

// Método principal
void Main()
{
    if (debug)
    {
        print(2, "debug");
        travar();
    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        fechar_atuador();
        abaixar_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_rampa_resgate();
        }
        travar();
    }
}