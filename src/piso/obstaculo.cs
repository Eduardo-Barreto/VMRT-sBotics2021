bool verifica_obstaculo(bool contar_update = true)
{
    if (contar_update && millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        if (angulo_atuador() >= 0 && angulo_atuador() < 88)
            mover_tempo(-200, 79);
        levantar_atuador();
        console_led(1, "<:POSSÍVEL OBSTÁCULO:>", "azul");
        int timeout = millis() + 1500;
        while (ultra(0) > 12)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (ultra(0) > 20 && millis() > timeout)
            {
                console_led(1, "<:OBSTÁCULO FALSO:>", "vermelho");
                parar();
                abaixar_atuador();
                return false;
            }
        }
        parar();
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        alinhar_angulo();
        alinhar_ultra(12);
        parar();
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        girar_direita(45);
        som("E3", 32);
        mover_tempo(300, 735);
        som("E3", 32);
        girar_esquerda(45);
        som("E3", 32);
        // 495
        timeout = millis() + 559;
        while (ultra(2) > 15)
        {
            if (millis() > timeout) { break; }
            mover(300, 300);
        }
        while (ultra(2) < 15)
        {
            if (millis() > timeout) { break; }
            mover(300, 300);
        }
        mover_tempo(300, 127);
        som("E3", 32);
        girar_esquerda(60);
        som("E3", 32);
        timeout = millis() + 495;
        while (millis() < timeout)
        {
            if (preto(0) || preto(1))
            {
                break;
            }
            mover(200, 200);
        }
        parar();
        som("D3", 32);
        mover_tempo(300, 335);
        som("E3", 32);
        float objetivo = converter_graus(eixo_x() + 45);
        while (!preto(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                break;
            }
            mover(1000, -1000);
        }
        delay(200);
        alinhar_angulo();
        tempo_correcao = millis() + 350;
        while (millis() < tempo_correcao)
        {
            if (toque())
            {
                break;
            }
            mover(-150, -150);
        }
        parar();
        som("D3", 32);
        som("MUDO", 16);
        som("D3", 32);
        alinhar_linha();
        abaixar_atuador();
        if (proximo(eixo_y(), 350, 3))
            levantar_atuador();
        update_obstaculo = millis() + 100;
        return true;
    }
    return false;
}
