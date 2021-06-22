import("setup/variaveis");
import("setup/utils");
import("setup/leituras");
import("setup/movimentacao");
import("piso/seguir_linha");
import("piso/encruzilhadas");
import("piso/obstaculo");
import("piso/elevados");
import("resgate/movimentacao");
import("resgate/rampa");
import("resgate/sair");
import("resgate/achar_saida");

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
        abaixar_atuador();
    }
    // Loop principal do programa
    while (!debug)
    {
        while (lugar == 0)
        {
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_rampa_resgate();
        }
        limpar_console();
        levantar_atuador();
        console_led(1, "<size=\"60\"><:SUBINDO A RAMPA!:></size>", "#28ade2");
        som("B2", 500);
        while (lugar == 1)
        {
            velocidade = 250;
            seguir_rampa();
            if ((eixo_y() > 355) || (eixo_y() < 5))
            {
                lugar = 2;
            }
        }
        limpar_console();
        while (lugar == 2)
        {
            sair();
            limpar_console();
            while (verde(0) || verde(1) || verde(2) || verde(3))
                mover(200, 200);
            delay(150);
            delay(64);
            parar();
            mover(200, 200);
            delay(16);
            parar();
            lugar = 3;
        }
        abaixar_atuador();
        delay(700);
        while (lugar == 3)
        {
            if (verifica_saida()) { return; }
            verifica_obstaculo();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
        }
    }

    // Loop para debug
    while (debug)
    {
        bot.onTF(-1000, 1000);
        delay(500);
        mover(-1000, 1000);
        delay(500);
        travar();
    }
}
