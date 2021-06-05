bool verifica_elevada()
{
    if (millis() < update_elevado)
    {
        return false;
    }
    if (proximo(eixo_y(), 340, 10))
    {
        if (ultra(1) < 40 && ultra(2) < 40)
        {
            lugar = "rampa resgate";
            return false;
        }
        print(2, "RAMPA");
        int tempo_gangorra = millis() + 1500;
        while (millis() < tempo_gangorra)
        {
            seguir_linha();
        }
        parar();
        delay(550);
        update_elevado = millis() + 2000;
        limpar_linha(2);
        return true;

    }
    return false;
}