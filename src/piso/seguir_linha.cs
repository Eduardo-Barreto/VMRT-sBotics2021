// Segue as linhas
void seguir_linha()
{
    print(1, $"Seguindo linha: {velocidade}");
    bc.TurnLedOff();
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        tempo_correcao = millis() + 210;
        while (millis() < tempo_correcao)
        {
            mover(1000, -1000);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                return;
            }
        }
        mover(-1000, 1000);
        delay(210);
        parar();

        // Confirma que está perdido
        print(1, "Perdi a linha...");
        led(255, 0, 0);
        som("F#", 64);
        som("", 16);
        som("F#", 64);
        // Vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() + 1750;
        while (millis() < tras)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                break;
            }
        }
        delay(150);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }

    // Está saindo da pista (detectou o azul do fim da arena)
    if (azul(1) || azul(2))
    {
        print(1, "Saí da arena...");
        led(255, 0, 0);
        som("B", 64);
        som("MUDO", 16);
        som("B", 64);
        // Calcula a diferença desde a última correção e vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() - ultima_correcao + 150;
        tempo_correcao = millis() + tras;
        while (millis() < tempo_correcao)
        {
            mover(-velocidade, -velocidade);
            if (cor(0) == "PRETO" || cor(1) == "PRETO" || cor(2) == "PRETO" || cor(3) == "PRETO")
            {
                break;
            }
        }
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }


    // Incremento da velocidade de acordo com o tempo
    if ((millis() > update_time) && (velocidade < velocidade_max))
    {
        update_time = millis() + 32;
        velocidade++;
    }

    // Área do seguidor===============================================================================

    // Se viu preto no sensor da direita
    if (preto1)
    {
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;


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
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    // Se viu preto no sensor da direita
    else if (preto2)
    {
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

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
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        mover(velocidade, velocidade);
    }
}