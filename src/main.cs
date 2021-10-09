import("setup/variaveis.cs");
import("setup/utils.cs");
import("setup/leituras.cs");
import("setup/movimentacao.cs");
import("piso/seguir_linha.cs");
import("piso/encruzilhadas.cs");
import("piso/obstaculo.cs");
import("piso/elevados.cs");
import("resgate/setup/variaveis.cs");
import("resgate/setup/movimentacao.cs");
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
        mover_tempo(300, 20000);
        print(1, "travou");
        delay(60);
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
            if (!pegou_kit && kit_frente())
            {
                parar();
                limpar_console();
                console_led(2, "<:KIT DE RESGATE IDENTIFICADO:>", "azul");
                while (kit_frente())
                {
                    mover(-250, -250);
                }
                mover_tempo(-300, 100);
                abrir_atuador();
                girar_baixo_atuador();
                abaixar_atuador();
                int init_time = millis();
                while (!tem_kit())
                {
                    mover(300, 300);
                }
                mover(300, 300);
                delay(192);
                fechar_atuador();
                girar_cima_atuador();
                levantar_atuador();
                parar();
                int kit_time = millis();
                mover_tempo(-300, (kit_time - init_time) / 2);
                limpar_console();
                parar();
                if (tem_kit())
                {
                    pegou_kit = true;
                }
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
