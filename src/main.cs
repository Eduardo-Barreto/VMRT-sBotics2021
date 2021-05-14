import("setup/variaveis");
import("setup/utils");
import("setup/leituras");
import("setup/movimentacao");
import("seguir_linha");
import("encruzilhadas");

bool debug = false;

void Main()
{
    calibrar();
    while (!debug)
    {
        verifica_calibrar();
        seguir_linha();
        verifica_verde();
    }

    float menor = 100;
    while (debug)
    {
        ler_cor();
        if (verde2 || verde3)
        {
            print(1, "CURVA VERDE - Esquerda");
            encoder(-300, 2);
            ajustar_linha();
            encoder(300, 2);
            delay(64);
            ler_cor();
            if (verde2 || verde3)
            {
                led(0, 255, 0);
                som("SOL", 100);
                while (!(tem_linha(0)))
                {
                    mover(190, 190);
                }
                som("LÁ", 100);
                while (cor(0) == "PRETO")
                {
                    mover(190, 190);
                }
                parar();
                som("SI", 100);
                encoder(300, 9);
                girar_esquerda(20);
                while(!tem_linha(2)){
                    mover(-1000, 1000);
                    if(Array.IndexOf(angulos_retos, eixo_x()) != -1){
                        break;
                    }
                }
                delay(200);
                ajustar_linha();
                encoder(-300, 2);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
            }
        }
        else
        {
            mover(200, 200);
        }
    }
}
