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
        while (lugar == "piso")
        {
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_rampa_resgate();
        }
        limpar_console();
        console_led(1, "<:SUBINDO A RAMPA!:>", "#28ade2");
        som("B2", 500);
        while (lugar == "rampa resgate")
        {
            velocidade = 250;
            seguir_rampa();
            if ((eixo_y() > 355) || (eixo_y() < 5))
            {
                lugar = "resgate";
            }
        }
        limpar_console();
        while (lugar == "resgate")
        {
            achar_saida();
            travar();
            limpar_console();
            while (verde(0) || verde(1) || verde(2) || verde(3))
                mover(200, 200);
            delay(64);
            parar();
            mover(200, 200);
            delay(16);
            parar();
            lugar = "percurso de saida";
        }
        abaixar_atuador();
        delay(700);
        while (lugar == "percurso de saida")
        {
            if (verifica_saida()) { encoder(300, 15); travar(); }
            verifica_obstaculo();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
        }
    }

    // Loop para debug
    while (debug)
    {
        limpar_console();
        alinhar_angulo();
        alinhar_ultra(124);
        alinhar_angulo();
        console_led(1, $"<:ESTOU ALINHADO VADIAS:> ({ultra(0)})", "vermelho");
        girar_direita(90);
        print(1, $"{ultra(1)} <> {ultra(2)}");
        travar();
    }
}
