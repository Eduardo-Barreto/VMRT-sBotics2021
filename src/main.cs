import("setup/variaveis.cs");
import("setup/utils.cs");
import("setup/leituras.cs");
import("setup/movimentacao.cs");
import("piso/seguir_linha.cs");
import("piso/encruzilhadas.cs");
import("piso/obstaculo.cs");
import("piso/elevados.cs");
import("resgate/setup/movimentacao.cs");
import("resgate/setup/variaveis.cs");
import("resgate/varredura.cs");



// Variáveis de controle para ligar/desligar o debug e console
bool debug = false;
bool console = true;
bool registro = true;

// Método principal
void Main()
{
    if (debug)
    {

    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        bot.ActuatorSpeed(150);
        levantar_atuador();
        fechar_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            if (kit_frente())
            {
                print(1, "identificou kit");
                mover_tempo(-300, 500);
                abrir_atuador();
                abaixar_atuador();
                while (!tem_kit())
                {
                    seguir_linha();
                }
                timeout = millis() + 1000;
                while (millis() < timeout)
                {
                    seguir_linha();
                }
                fechar_atuador();
                levantar_atuador();
            }
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_fita_cinza();
        }
        limpar_console();
        print(2, "Sala de salvamento identificada");
        mover_tempo(300, 2043);
        varredura();
        travar();
    }
}