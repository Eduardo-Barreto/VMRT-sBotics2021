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
        short maior_diferenca = 0;
        short ultimo_valor = 0;
        for (short i = 0; i < 360; i++)
        {
            if (i > 0)
            {
                girar_direita(1);
                short valor = ultra(2);
                short diferenca = valor - ultimo_valor;
                if (diferenca > maior_diferenca)
                {
                    maior_diferenca = diferenca;
                }
                ultimo_valor = valor;
                print(1, maior_diferenca);
            }
        }
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
                mover_tempo(-300, 500);
                abrir_atuador();
                girar_baixo_atuador();
                abaixar_atuador();
                int init_time = millis();
                while (!tem_kit())
                {
                    mover(300, 300);
                }
                mover(300, 300);
                delay(287);
                fechar_atuador();
                girar_cima_atuador();
                levantar_atuador();
                parar();
                int kit_time = millis();
                mover_tempo(-300, (kit_time - init_time) - 286);
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
