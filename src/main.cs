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
        for (; ; )
        {
            kit_frente();
        }
    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        bot.ActuatorSpeed(150);
        abaixar_atuador();
        abrir_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            if (pegou_kit == false && tem_kit())
            {
                timeout = millis() + 1000;
                while (millis() < timeout)
                {
                    seguir_linha();
                }
                fechar_atuador();
                levantar_atuador();
                parar();
                pegou_kit = true;
            }
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_fita_cinza();
        }
        print(1, "detectou");
        travar();
    }
}