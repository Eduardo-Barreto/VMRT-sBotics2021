bool verifica_obstaculo(bool contar_update = true)
{
    if (contar_update && millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        limpar_console();
        parar();
        mover_tempo(-200, 79);
        if (!tem_kit())
        {
            fechar_atuador();
            levantar_atuador();
        }
        console_led(2, "<:POSSÍVEL OBSTÁCULO:>", "azul");
        timeout = millis() + 1500;
        while (ultra(0) > 12)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (ultra(0) > 20 && millis() > timeout)
            {
                console_led(1, "<:OBSTÁCULO FALSO:>", "vermelho");
                parar();
                if (!tem_kit())
                {
                    abrir_atuador();
                    abaixar_atuador();
                }
                return false;
            }
        }
        parar();
        limpar_console();
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        alinhar_angulo();
        alinhar_ultra(12);

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
            mover_tempo(-150, 159);
            alinhar_linha();
            if (ultra(0) > 35 && !tem_kit())
            {
                abrir_atuador();
                abaixar_atuador();
                update_obstaculo = millis() + 100;
            }
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
        }

        print(2, "Verificando desvio à direita...");
        girar_direita(45);
        mover_tempo(300, 367);
        girar_esquerda(15);

        timeout = millis() + 239;
        while (millis() < timeout)
        {
            mover(250, 250);
            if (preto(1) || preto(2))
            {
                parar();
                print(2, "Desvio à direita confirmado");
                girar_direita(15);
                mover_tempo(300, 159);
                alinhar_pos_obstaculo();
                return true;
            }
        }
        parar();
        print(2, "Verificando desvio reto...");
        girar_esquerda(5);
        mover_tempo(300, 303);
        girar_esquerda(25);
        alinhar_angulo();
        mover_tempo(300, 399);
        girar_esquerda(60);

        timeout = millis() + 399;
        while (millis() < timeout)
        {
            mover(250, 250);
            if (preto(1) || preto(2))
            {
                parar();
                print(2, "Desvio reto confirmado");
                alinhar_pos_obstaculo();
                return true;
            }
        }
        parar();

        print(2, "Desvio à esquerda");
        girar_esquerda(30);
        alinhar_angulo();
        mover_tempo(300, 399);
        girar_esquerda(45);
        timeout = millis() + 399;
        while (!(preto(1) || preto(2)))
        {
            mover(250, 250);
            if (millis() > timeout)
            {
                break;
            }
        }
        parar();
        mover_tempo(300, 127);
        alinhar_pos_obstaculo();
        return true;

        travar();
    }
    return false;
}