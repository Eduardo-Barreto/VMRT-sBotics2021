bool verifica_obstaculo()
{
    if (millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        alinhar_angulo();
        levantar_atuador();
        console_led(1, "<:OBSTÁCULO:>", "azul");
        encoder(300, 10);
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        girar_direita(45);
        som("E3", 32);
        encoder(300, 20);
        som("E3", 32);
        girar_esquerda(45);
        som("E3", 32);
        encoder(300, 15);
        som("E3", 32);
        girar_esquerda(45);
        som("E3", 32);
        int timeout_obstaculo = millis() + 600;
        while (!preto(0) && !preto(1))
        {
            if(millis() > timeout_obstaculo){
                break;
            }
            mover(200, 200);
        }
        parar();
        som("D3", 32);
        encoder(300, 10);
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
        ajustar_linha();
        abaixar_atuador();
        if (proximo(eixo_y(), 350, 3))
            levantar_atuador();
        update_obstaculo = millis() + 100;
        return true;
    }
    return false;
}
