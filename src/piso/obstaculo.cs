bool verifica_obstaculo()
{
    if (millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        alinhar_angulo();
        print(1, "OBSTÁCULO");
        led(40, 153, 219);
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
        encoder(300, 25);
        som("E3", 32);
        girar_esquerda(45);
        som("E3", 32);
        while (!preto(0) && !preto(1))
        {
            mover(200, 200);
        }
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
        update_obstaculo = millis() + 100;
        return true;
    }
    return false;
}
