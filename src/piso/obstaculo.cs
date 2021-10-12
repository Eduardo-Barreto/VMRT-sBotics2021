bool verifica_obstaculo(bool contar_update = true)
{
    if (contar_update && millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        som("B1", 64);
        som("D2", 64);
        console_led(2, "<:POSSÍVEL OBSTÁCULO:>\n\n", "azul");
        timeout = millis() + 1167;
        while (ultra(0) > 12)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (ultra(0) > 20 && millis() > timeout)
            {
                console_led(1, "<:OBSTÁCULO FALSO:>", "vermelho");
                parar();
                return false;
            }
        }
        parar();
        alinhar_angulo();
        limpar_console();
        som("G2", 64);
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        parar();
        while (ultra(0) > 12)
        {
            mover(75, 75);
        }
        while (ultra(0) < 12)
        {
            mover(-75, -75);
        }
        while (ultra(0) > 12)
        {
            mover(75, 75);
        }
        while (ultra(0) < 12)
        {
            mover(-75, -75);
        }
        parar();

        void alinhar_pos_obstaculo()
        {
            mover_tempo(300, 335);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            alinhar_angulo();
            mover_tempo(-150, 1000);
            alinhar_linha();
            som("D2", 64);
            update_obstaculo = millis() + 50;
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
        }

        void finalizar_desvio_direita()
        {
            som("F#2", 64);
            print(2, "Desvio à direita confirmado!");
            girar_direita(15);
            som("E2", 128);
            mover_tempo(300, 191);
            alinhar_pos_obstaculo();
        }

        void finalizar_desvio_reto()
        {
            som("D2", 64);
            print(2, "Desvio reto confirmado!");
            mover_tempo(300, 127);
            alinhar_pos_obstaculo();
        }

        void finalizar_desvio_esquerda()
        {
            print(2, "Desvio à esquerda confirmado!");
            mover_tempo(300, 127);
            alinhar_pos_obstaculo();
        }


        print(2, "Verificando desvio à direita...");
        girar_direita(50);
        mover_tempo(300, 319);

        int objetivo = 0;
        for (int i = 0; i < 5; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 10));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_direita();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_direita();
                    return true;
                }
            }
            parar();
        }


        print(2, "Verificando desvio reto...");
        som("F#2", 64);

        for (int i = 0; i < 3; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 15));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
        }

        for (int i = 0; i < 3; i++)
        {
            objetivo = (int)(converter_graus(eixo_x() - 10));
            while (!proximo(eixo_x(), objetivo))
            {
                mover(-1000, 1000); ;
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
            objetivo = millis() + 159;
            while (millis() < objetivo)
            {
                mover(300, 300);
                if (preto(1) || preto(2))
                {
                    finalizar_desvio_reto();
                    return true;
                }
            }
            parar();
        }

        print(2, "Verificando desvio à esquerda...");
        som("E2", 64);
        alinhar_angulo();
        mover_tempo(300, 239);
        girar_esquerda(45);

        objetivo = millis() + 271;
        while (millis() < objetivo)
        {
            mover(300, 300);
            if (preto(1) || preto(2))
            {
                break;
            }
        }
        finalizar_desvio_esquerda();
        parar();
        return true;

    }

    return false;
}