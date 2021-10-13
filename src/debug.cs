void passar_vassoura()
{
    void chegar_final()
    {
        /*
            Chega ao final perto da parede e retorna o porquê da parada
        */

        // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
        short angulo_inicial = eixo_x();
        // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
        int tempo_check = millis() + 200;
        // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
        bool flag_check = false;
        // Flag de verificação configurada como verdadeiro, se ele parar por algo que não foi parede, ela é trocada
        bool parede = true;
        string motivo = "ultrassonico";

        // Enquanto o ultrassônico não identifica parede
        while (ultra(0) > 25)
        {
            // Move o robô
            mover(250, 250);
            if (!flag_check && millis() > tempo_check)
            {
                // Se a flag era falsa e já passou o tempo inicial
                // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
                flag_check = true;
            }

            if (flag_check && forca_motor() < 0.3)
            {
                // Se a flag ja for verdadeira e a força atual for menor que 0.3
                // Para o loop
                motivo = "forca";
                break;
            }
            if (flag_check && !proximo(eixo_x(), angulo_inicial, 3))
            {
                // Se a flag ja for verdadeira e o angulo atual for muito diferente do angulo inicial
                motivo = "direcao";
                break;
            }
            if (fita_cinza(0) && fita_cinza(1) && fita_cinza(2) && fita_cinza(3))
            {
                // Se identificar a fita cinza
                // Declara que não foi a parede que parou
                parede = false;
                motivo = "cinza";
                // Para o loop
                break;
            }
            if (verde(0) || verde(1) || verde(2) || verde(3))
            {
                // Se identificar a fita cinza
                // Declara que não foi a parede que parou
                parede = false;
                motivo = "verde";
                // Para o loop
                break;
            }
        }
        // Volta o atuador para sua posição inicial e para o robô
        fechar_atuador();
        girar_cima_atuador();
        levantar_atuador();
        parar();
        print(2, $"Robô parado por: {motivo}");
        // Se alinha novamente caso não tenha parado pela parede
        if (motivo == "cinza")
        {
            while ((!fita_cinza(0) && !fita_cinza(1) && !fita_cinza(2) && !fita_cinza(3)))
            {
                mover(-250, -250);
            }
            while ((fita_cinza(0) || fita_cinza(1) || fita_cinza(2) || fita_cinza(3)))
            {
                mover(-250, -250);
            }
        }
        if (motivo == "verde")
        {
            while ((!verde(0) && !verde(1) && !verde(2) && !verde(3)))
            {
                mover(-250, -250);
            }
            while ((verde(0) || verde(1) || verde(2) || verde(3)))
            {
                mover(-250, -250);
            }
        }
        delay(239);
        parar();

    }

    // Alinha para começar a varredura
    objetivo_direita(converter_graus(eixo_x() + 90));
    preparar_atuador();
    chegar_final();
    // Após ir até o fim da direita, se alinha para começar o loop
    objetivo_esquerda(converter_graus(eixo_x() - 90));
    mover_tempo(-300, 300);
    objetivo_esquerda(converter_graus(eixo_x() - 90));

    // Loop para varrer os cantos
    for (int i = 0; i < 3; i++)
    {
        preparar_atuador();
        alinhar_angulo();
        chegar_final();
        mover_tempo(300, 191);
        alinhar_angulo();
        if (luz(4) < 5)
        {
            alinhar_angulo();
            objetivo_direita(converter_graus(eixo_x() + 45));
            alinhar_ultra(35);
            mover_tempo(300, 239);
            objetivo_direita(converter_graus(eixo_x() + 20));
            mover_tempo(300, 191);
            objetivo_direita(converter_graus(eixo_x() + 25));
            mover_tempo(300, 639);
        }
        else
        {
            objetivo_direita(converter_graus(eixo_x() + 90));
            preparar_atuador();
        }
    }
    travar();
}
