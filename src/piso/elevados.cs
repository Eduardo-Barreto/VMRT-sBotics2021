bool verifica_elevada()
{

    /* 
    
        Quando a inclinação for próxima de 350
            Levanta o atuador
            Sobe e para o tempo suficiente pra possível gangorra
            Abaixa o atuador e retorna

    */
    if (millis() < update_rampa)
        return false;

    if (proximo(eixo_y(), 350))
    {
        parar();
        levantar_atuador();
        int tempo_gangorra = millis() + 2000;
        while (millis() < tempo_gangorra)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (verifica_rampa_resgate())
                return true;
        }
        parar();
        delay(550);
        abaixar_atuador();
        update_rampa = millis() + 2000;
        return true;
    }
    return false;

}

bool verifica_rampa_resgate()
{
    if ((proximo(eixo_y(), 340, 10)) && (ultra(1) < 40 && ultra(2) < 40))
    {
        lugar = "rampa resgate";
        return true;
    }
    return false;
}
