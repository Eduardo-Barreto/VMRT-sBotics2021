void verifica_obstaculo()
{
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
        encoder(300, 20);// ACHAR LINHAA!
        som("E3", 32);
        float objetivo = converter_graus(eixo_x() + 45);
        while (!tem_linha(1) && !vermelho(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                break;
            }
            mover(1000, -1000);
        }
        delay(200);
        alinhar_angulo();
        tempo_correcao = millis() + 500;
        while (millis() < tempo_correcao)
        {
            mover(-150, -150);
            if (toque())
            {
                break;
            }
        }
        som("D3", 32);
        som("MUDO", 16);
        som("D3", 32);
        parar();
        ajustar_linha();
    }
}
