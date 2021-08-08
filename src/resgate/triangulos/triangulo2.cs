void triangulo2()
{
    alinhar_angulo();
    abrir_atuador();
    abaixar_atuador();
    bool alinhou_angulo_meio = false;
    ler_ultra();
    while (ultra_frente > 30)
    {
        ler_ultra();
        mover(250, 250);

        // Alinhhar o ângulo no meio da arena
        if (!alinhou_angulo_meio && ultra_frente < 140)
        {
            // Se ainda não alinhou e o robô está mais ou menos no meio
            // Alinha e indica que ja alinhou
            alinhar_angulo();
            alinhou_angulo_meio = true;
        }

        // Se encontra vítima no atuador indo para frente
        if (tem_vitima())
        {
            limpar_console();
            print(1, "Encontrei vítima no meio do caminho");
            fechar_atuador();
            levantar_atuador();
            print(2, "Alinhando ao meio");
            alinhar_ultra(124);
            meio_triangulo();
            print(2, "Voltando até a parede");
            mover_tempo(-300, 2000);
            preparar_atuador();
            alinhou_angulo_meio = false;
            limpar_console();
        }

        // Se já saiu do alcance do triângulo e encontra algo na direita
        if (ultra_frente < 160 && ultra_direita < 122)
        {
            limpar_console();
            print(1, $"Vítima encontrada na direita ({ultra_direita})zm");
            delay(63);
            parar();
            print(2, "Indo buscar");
            // Levanta o atuador pra nao bater na vitima
            fechar_atuador();
            levantar_atuador();
            // Gira em direção da vítima
            girar_direita(90);
            // Prepara o atuador para pegar
            mover_tempo(-300, 447);
            alinhar_angulo();
            abrir_atuador();
            abaixar_atuador();
            init_time = millis();
            while (ultra(0) > 30)
            {
                // Vai pra frente
                mover(300, 300);
                // Se identificar vítima, espera um pouco e sai do loop
                if (tem_vitima())
                {
                    delay(127);
                    break;
                }
                if (millis() > init_time + 1695)
                {
                    break;
                }
            }
            int tempo_voltar = millis() - init_time;
            // Retorna o atuador e se alinha no meio
            fechar_atuador();
            levantar_atuador();
            if (ultra(0) > 150)
            {
                mover_tempo(-300, tempo_voltar);
            }
            else
            {
                alinhar_ultra(124);
            }



            alinhar_angulo();
            // Se tiver vítima, coloca na área segura
            if (tem_vitima())
            {
                limpar_console();
                print(1, "Peguei! Levando à área segura");
                girar_esquerda(90);
                alinhar_angulo();
                alinhar_ultra(124);
                meio_triangulo();
            }
            // Se não, só avisa
            else
            {
                print(1, "Ih rapaz ela fugiu...");
                if (ultra(0) > 200)
                {
                    girar_direita(180);
                    alinhar_ultra(124);
                    girar_direita(90);
                }
                else
                {
                    girar_esquerda(90);
                }
            }
            print(2, "Voltando até a parede");
            mover_tempo(-300, 2000);
            preparar_atuador();
            alinhou_angulo_meio = false;
            limpar_console();
        }



        // Se já saiu do alcance do triângulo e encontra algo na esquerda
        if (ultra_esquerda < 122)
        {
            limpar_console();
            print(1, $"Vítima encontrada na esquerda ({ultra_esquerda})zm");
            delay(63);
            parar();
            print(2, "Indo buscar");
            // Levanta o atuador pra nao bater na vitima
            fechar_atuador();
            levantar_atuador();
            // Gira em direção da vítima
            girar_esquerda(90);
            // Prepara o atuador para pegar
            mover_tempo(-300, 447);
            alinhar_angulo();
            abrir_atuador();
            abaixar_atuador();
            init_time = millis();
            while (ultra(0) > 30)
            {
                // Vai pra frente
                mover(300, 300);
                // Se identificar vítima, espera um pouco e sai do loop
                if (tem_vitima())
                {
                    delay(127);
                    break;
                }
                if (millis() > init_time + 1695)
                {
                    break;
                }
            }
            int tempo_voltar = millis() - init_time;
            // Retorna o atuador e se alinha no meios
            fechar_atuador();
            levantar_atuador();
            alinhar_angulo();

            if (ultra(0) > 150)
            {
                mover_tempo(-300, tempo_voltar);
            }
            else
            {
                alinhar_ultra(124);
            }

            // Se tiver vítima, coloca na área segura
            if (tem_vitima())
            {
                limpar_console();
                print(1, "Peguei! Levando à área segura");
                girar_direita(90);
                alinhar_angulo();
                alinhar_ultra(124);
                meio_triangulo();
            }
            // Se não, só avisa
            else
            {
                print(1, "Ih rapaz ela fugiu...");

                if (ultra(0) > 200)
                {
                    girar_direita(180);
                    alinhar_ultra(124);
                    girar_esquerda(90);
                }
                else
                {
                    alinhar_ultra(124);
                    girar_direita(90);
                }
            }
            print(2, "Voltando até a parede");
            mover_tempo(-300, 2000);
            preparar_atuador();
            alinhou_angulo_meio = false;
            limpar_console();
        }


        print(1, "Procurando vítimas...");
        print(2, $"Direita: {ultra_direita} | Esquerda: {ultra_esquerda}");
    }




    // TERMINOU DE IR PRA FRENTE
    fechar_atuador();
    levantar_atuador();
    if (tem_vitima())
    {
        limpar_console();
        print(1, "Encontrei vítima no fim do caminho");
        print(2, "Alinhando ao meio");
        alinhar_ultra(124);
        print(2, "Girando para o triângulo");
        girar_direita(135);
        print(2, "Indo para o triângulo");
        while (ultra(0) > 80)
        {
            mover(300, 300);
        }
        mover_tempo(250, 500);
        print(2, "Entregando vítima");
        entregar_vitima();
        print(1, "Voltando à busca");
        print(2, "Indo ao meio");
        while (ultra(0) < 175)
        {
            mover(-300, -300);
        }
        print(2, "Alinhando...");
        girar_esquerda(45);
        alinhar_angulo();
        alinhar_ultra(124);
        girar_esquerda(90);
        limpar_console();
        print(1, "Fim da varredura, saindo da sala de salvamento");
        alinhar_ultra(124);
    }
    else
    {
        limpar_console();
        print(1, "Fim da varredura, saindo da sala de salvamento");
        alinhar_ultra(124);
    }

    if (direcao_saida == 1)//aaaaaaaaaa
    {
        alinhar_angulo();
        girar_direita(45);
        while (ultra(0) > 40)
        {
            mover(300, 300);
        }
        alinhar_ultra(35);
        girar_esquerda(45);
        alinhar_angulo();
        alcancar_saida();
    }
    else
    {
        alinhar_angulo();
        girar_esquerda(90);
        while (ultra(0) > 30)
        {
            mover(300, 300);
        }
        alinhar_ultra(23);
        girar_esquerda(90);
        alinhar_angulo();
        alcancar_saida();
    }
}
