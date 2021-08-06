void seguir_rampa()
{
    for (; ; )
    {
        ler_cor();

        if ((eixo_y() <= 1) || ultra(1) > 50)
        {
            lugar = 2;
            parar();
            return;
        }

        if (preto0 || preto1)
        {
            // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
            tempo_correcao = millis() + 210;
            while (tempo_correcao > millis())
            {
                if (branco(1) || preto(2))
                {
                    break;
                }
                mover(1000, -1000);
            }
            // Vai para a frente por um pequeno tempo e atualiza a última correção
            mover(300, 300);
            delay(5);
        }

        // Se viu preto no sensor da direita
        else if (preto2 || preto3)
        {
            // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
            tempo_correcao = millis() + 210;
            while (tempo_correcao > millis())
            {
                if (branco(2) || preto(1))
                {
                    break;
                }
                mover(-1000, 1000);
            }
            // Vai para a frente por um pequeno tempo e atualiza a última correção
            mover(300, 300);
            delay(5);
        }

        // Se está certo na linha só vai para frente com a velocidade atual
        else
        {
            mover(300, 300);
        }
    }
}
