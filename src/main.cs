import("setup/variaveis.cs");
import("setup/utils.cs");
import("setup/leituras.cs");
import("setup/movimentacao.cs");
import("piso/seguir_linha.cs");
import("piso/encruzilhadas.cs");
import("piso/obstaculo.cs");
import("piso/elevados.cs");
import("resgate/setup/movimentacao.cs");
import("resgate/rampa.cs");
import("resgate/achar_saida.cs");
import("resgate/triangulos/triangulo1.cs");
import("resgate/triangulos/triangulo2.cs");
import("resgate/triangulos/triangulo3.cs");


// Variáveis de controle para ligar/desligar o debug e console
bool debug = false;
bool console = true;

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
        while (lugar == 1)
        {
            limpar_console();
            levantar_atuador();
            console_led(1, "<size=\"60\"><:SUBINDO A RAMPA!:></size>", "azul");
            som("B2", 500);
            seguir_rampa();
        }
        console_led(3, "<:Local atual: RESGATE:>", "cinza claro", false);
        while (lugar == 2)
        {
            achar_saida();
            travar();
        }
        console_led(3, "<:Local atual: PERCURSO DE SAÍDA:>", "cinza claro", false);
        while (lugar == 3)
        {
            print_luz_marker();
            verifica_saida();
            verifica_obstaculo();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
        }
    }
}