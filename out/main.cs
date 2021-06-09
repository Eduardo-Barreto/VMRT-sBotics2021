// Declaração das variáveis principais de todo o projeto, separadas por tipos
float saida1 = 0,
        saida2 = 0,
        media_meio = 0,
        media_fora = 0,
        direcao_saida = 0,
        direcao_triangulo = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0;

int velocidade_padrao = 185,
        velocidade = 180,
        velocidade_max = 220,
        update_time = 16,
        tempo_correcao = 0,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0;

bool preto0 = false,
        preto1 = false,
        preto2 = false,
        preto3 = false,

        verde0 = false,
        verde1 = false,
        verde2 = false,
        verde3 = false,

        preto_curva_dir = false,
        preto_curva_esq = false;

int[] angulos_retos = { 0, 90, 180, 270 };

string lado_ajuste = "0",
        lugar = "piso";
// Comandos úteis para todo o código

float map(float val, float minimo, float maximo, float minimoSaida, float maximoSaida)
{
    //"mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo, float sensibilidade = 1)
{
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo - sensibilidade && atual < objetivo + sensibilidade);
}

float converter_graus(float graus)
{
    // converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
    return (graus % 360 + 360) % 360;
}
// Métodos de leitura e outros

int millis() => (int)(bc.Timer());
bool toque() => (bc.Touch(0));
string cor(int sensor) => bc.ReturnColor(sensor);
int luz(byte sensor) => (int)bc.Lightness(sensor);
int ultra(byte sensor) => (int)bc.Distance(sensor);
float eixo_x() => bc.Compass();
float eixo_y() => bc.Inclination();
float angulo_atuador() => bc.AngleActuator();
float angulo_giro_atuador() => bc.AngleScoop();
void delay(int milissegundos) => bc.Wait(milissegundos);

void som(string nota, int tempo) => bc.PlayNote(0, nota, tempo);
void led(byte R, byte G, byte B) => bc.TurnLedOn(R, G, B);

string[] consoleLines = { "", "", "", "" };

void print(int linha, object texto) { if (console) bc.Print(linha - 1, "<align=center>" + texto.ToString() + "</align>"); }

void limpar_console() => bc.ClearConsole();
void limpar_linha(int linha) => bc.Print(linha - 1, "");

bool tem_linha(int sensor) => (bc.returnRed(sensor) < 24);

bool vermelho(int sensor)
{
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 66, media_verde = 16, media_azul = 16;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)));
}

bool verde(int sensor)
{
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96) || cor(sensor) == "VERDE");
}

bool preto(int sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((bc.lightness(sensor) < media_meio) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((bc.lightness(sensor) < 40) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    return false;
}

bool branco(int sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((bc.lightness(sensor) > media_meio) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((bc.lightness(sensor) > media_fora) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    return false;
}

void calibrar()
{
    ajustar_linha(true);
    media_meio = (luz(1) + luz(2)) / 4.2f;
    media_fora = (luz(0) + luz(3)) / 4.2f;

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

    print(3, $"<color=#4c4d53>calibração: {media_meio} || {media_fora}</color>");
}

void verifica_calibrar()
{
    if (proximo(eixo_x(), saida1))
    {
        calibrar();
    }

    else if (proximo(eixo_x(), saida2))
    {
        calibrar();
    }
}

void ler_cor()
{
    preto0 = preto(0);
    preto1 = preto(1);
    preto2 = preto(2);
    preto3 = preto(3);

    verde0 = verde(0);
    verde1 = verde(1);
    verde2 = verde(2);
    verde3 = verde(3);

    preto_curva_dir = (preto(0));
    preto_curva_esq = (preto(3));
}

void ler_ultra()
{
    ultra_frente = ultra(0);
    ultra_direita = ultra(1);
    ultra_esquerda = ultra(2);
}

bool angulo_reto()
{
    foreach (int angulo_verificado in angulos_retos)
    {
        if (proximo(eixo_x(), angulo_verificado))
        {
            return true;
        }
    }
    return false;
}
// Métodos de movimentação e outros

void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void encoder(int velocidade, float rotacoes) => bc.MoveFrontalRotations(velocidade, rotacoes);
void parar() { bc.MoveFrontal(0, 0); delay(10); }
void travar() { bc.MoveFrontal(0, 0); delay(999999999); }

// Curva para a esquerda em graus
void girar_esquerda(int graus)
{
    float objetivo = converter_graus(eixo_x() - graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Curva para a direita em graus
void girar_direita(int graus)
{
    float objetivo = converter_graus(eixo_x() + graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Gira para a esquerda até um objetivo (bússola)
void objetivo_esquerda(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Alinha o robô no ângulo reto mais próximo
void alinhar_angulo()
{
    led(255, 255, 0);
    print(2, "Alinhando robô");

    int alinhamento = 0;
    float angulo = eixo_x();

    if (angulo_reto())
    {
        return;
    }

    if ((angulo > 315) || (angulo <= 45))
    {
        alinhamento = 0;
    }
    else if ((angulo > 45) && (angulo <= 135))
    {
        alinhamento = 90;
    }
    else if ((angulo > 135) && (angulo <= 225))
    {
        alinhamento = 180;
    }
    else if ((angulo > 225) && (angulo <= 315))
    {
        alinhamento = 270;
    }

    angulo = eixo_x();

    if ((alinhamento == 0) && (angulo > 180))
    {
        objetivo_direita(alinhamento);
    }
    else if ((alinhamento == 0) && (angulo < 180))
    {
        objetivo_esquerda(alinhamento);
    }
    else if (angulo < alinhamento)
    {
        objetivo_direita(alinhamento);
    }
    else if (angulo > alinhamento)
    {
        objetivo_esquerda(alinhamento);
    }

    limpar_linha(2);
}

// Ajusta os sensores na linha preta
void ajustar_linha(bool por_luz = false)
{
    led(255, 255, 0);

    if (por_luz)
    {
        if (luz(0) < 30 && luz(1) < 30 && luz(2) < 30 && luz(3) < 30)
        {
            mover(200, 200);
            delay(200);
            return;
        }
        tempo_correcao = millis() + 150;
        while (luz(0) < 30 && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(1) < 30 && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(3) < 30 && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(2) < 30 && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
    }
    else
    {
        tempo_correcao = millis() + 150;
        while (cor(0) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(1) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(3) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(2) == "PRETO" && millis() < tempo_correcao)
            bc.onTF(1000, -1000);
    }

    delay(64);
    parar();
}

void alinhar_ultra(int distancia)
{
    while (ultra(0) > distancia)
    {
        mover(300, 300);
    }
    while (ultra(0) < distancia)
    {
        mover(-300, -300);
    }
    while (ultra(0) > distancia)
    {
        mover(200, 200);
    }
    while (ultra(0) < distancia)
    {
        mover(-200, -200);
    }
    parar();
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bc.ActuatorSpeed(150);
    bc.ActuatorUp(100);
    if (angulo_atuador() >= 0 && angulo_atuador() < 88)
    {
        bc.ActuatorSpeed(150);
        bc.ActuatorUp(600);
    }
}

void abaixar_atuador()
{
    if (angulo_atuador() > 5)
    {
        bc.ActuatorSpeed(150);
        bc.ActuatorDown(600);
    }
}
bool verifica_saida()
{
    if (lugar == "percurso de saida")
    {
        return (vermelho(0)) || (vermelho(1)) || (vermelho(2)) || (vermelho(3));
    }
    // Está saindo da pista (detectou o fim da arena)
    else if (vermelho(1) || vermelho(2))
    {
        print(1, "<color=#c93432>Saí da arena...</color>");
        led(255, 0, 0);
        som("B3", 64);
        som("MUDO", 16);
        som("B3", 64);
        // Calcula a diferença desde a última correção e vai para trás até encontrar uma linha ou estourar o tempo
        mover(-velocidade, -velocidade);
        delay(150);
        int tras = millis() - ultima_correcao;
        tempo_correcao = millis() + tras;
        while (millis() < tempo_correcao)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                if (vermelho(1) || vermelho(2)) { break; }
            }
        }
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        return true;
    }
    else
    {
        return false;
    }
}

// Segue as linhas
void seguir_linha()
{
    if (verifica_saida()) { return; }
    if (verifica_curva()) { return; }
    print(1, $"Seguindo linha: <color=#3ea7fa>{velocidade}</color>");
    bc.TurnLedOff();
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        float objetivo = (lado_ajuste == "d") ? (converter_graus(eixo_x() + 10)) : (converter_graus(eixo_x() - 10));
        while (!proximo(eixo_x(), objetivo))
        {
            if (lado_ajuste == "d")
                mover(1000, -1000);
            else
                mover(-1000, 1000);

            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                return;
            }
        }
        if (lado_ajuste == "d")
            girar_esquerda(10);
        else
            girar_direita(10);

        parar();

        // Confirma que está perdido
        print(1, "<b><color=#c93432>Perdi a linha...</color></b>");
        led(255, 0, 0);
        som("F#3", 64);
        som("MUDO", 16);
        som("F#3", 64);
        // Vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() + 1750;
        while (millis() < tras)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                break;
            }
        }
        delay(150);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }

    // Incremento da velocidade de acordo com o tempo
    if ((millis() > update_time) && (velocidade < velocidade_max))
    {
        update_time = millis() + 32;
        velocidade++;
    }

    // Área do seguidor===============================================================================

    // Se viu preto no sensor da direita
    if (preto1)
    {
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(1) || preto(2))
            {
                break;
            }
            mover(1000, -1000);
        }
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = "d";
    }

    // Se viu preto no sensor da direita
    else if (preto2)
    {
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(2) || preto(1))
            {
                break;
            }
            mover(-1000, 1000);
        }
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = "e";
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        verifica_curva();
        mover(velocidade, velocidade);
    }
}
// Verificação do beco sem saída
bool beco()
{
    // Para, lê as cores e verifica se está na condição do beco
    parar();
    delay(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        ajustar_linha();
        delay(64);
        ler_cor();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            // Feedback visual e sonoro para indicar que entrou na condição
            print(1, "BECO SEM SAÍDA");
            led(0, 255, 0);
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        return false;
    }
    return false;
}

// Verificação das condições de verde
bool verifica_verde()
{
    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "<b><color=#248f75>CURVA VERDE - Direita</color></b>");
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        ajustar_linha();
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F3", 100);
            tempo_correcao = millis() + 150;
            while (!(tem_linha(1)))
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            som("G3", 100);
            tempo_correcao = millis() + 150;
            while (cor(1) == "PRETO")
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            parar();
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_direita(25);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }

    // Verifica os sensores da direita
    else if (verde2 || verde3)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "<b><color=#248f75>CURVA VERDE - Esquerda</color></b>");
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        ajustar_linha();
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F3", 100);
            tempo_correcao = millis() + 150;
            while (!(tem_linha(2)))
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            som("G3", 100);
            tempo_correcao = millis() + 150;
            while (cor(2) == "PRETO")
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            parar();
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_esquerda(25);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}

// Verificações de curvas no preto
bool verifica_curva()
{
    // Atualiza leituras de cores, verifica se está no verde e depois no preto
    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        parar();
        delay(64);
        ler_cor();
        if (vermelho(0)) { return false; }
        if (preto_curva_esq)
        {
            encoder(200, 3);
            return false;
        }
        if (verifica_saida()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        // Confirmações visuais e sonoras de que entrou na condição da curva
        print(1, "CURVA PRETO - Direita");
        led(0, 0, 0);
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        // Confirmada a curva, gira até encontrar uma linha ou passar do ângulo máximo
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) && !vermelho(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                /* Se chegar ao ângulo máximo, é uma curva com um gap no final
                Se alinha e arruma a curva de 90 somente com a referência de graus*/
                tempo_correcao = millis() + 1000;
                for (int i = 0; i < 10; i++)
                {
                    encoder(-300, 0.2f);
                    if (millis() > tempo_correcao)
                    {
                        break;
                    }
                }
                mover(-1000, 1000);
                delay(650);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(1000, -1000);
        }
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha(true);
        ajustar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar();
        delay(64);
        ler_cor();
        if (vermelho(3)) { return false; }
        if (preto_curva_dir)
        {
            encoder(200, 3);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Esquerda");
        led(0, 0, 0);
        som("C3", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) && !vermelho(2))
        {
            ler_cor();
            if (proximo(eixo_x(), objetivo))
            {
                tempo_correcao = millis() + 1000;
                for (int i = 0; i < 10; i++)
                {
                    encoder(-300, 0.2f);
                    if (millis() > tempo_correcao)
                    {
                        break;
                    }
                }
                mover(1000, -1000);
                delay(650);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(-1000, 1000);
        }
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha(true);
        ajustar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }
    else
    {
        return false;
    }
}
bool verifica_obstaculo()
{
    if (millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        alinhar_angulo();
        levantar_atuador();
        print(1, "OBSTÁCULO");
        led(40, 153, 219);
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
        abaixar_atuador();
        if (proximo(eixo_y(), 350, 3))
            levantar_atuador();
        update_obstaculo = millis() + 100;
        return true;
    }
    return false;
}
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
void seguir_rampa()
{
    ler_cor();

    if (preto0 || preto1)
    {
        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(1) || preto(2))
            {
                break;
            }
            mover(1000, -1000);
        }
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
    }

    // Se viu preto no sensor da direita
    else if (preto2 || preto3)
    {
        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(2) || preto(1))
            {
                break;
            }
            mover(-1000, 1000);
        }
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        mover(velocidade, velocidade);
    }
}void alcancar_saida()
{
    mover(300, 300);
    delay(500);
    mover(-300, -300);
    delay(550);
    parar();
    abaixar_atuador();
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
        mover(300, 300);
    limpar_console();
    print(2, "Saindo!");
    som("C1", 100);
}

void sair()
{

    alinhar_angulo();
    bc.TurnLedOff();

    print(1, "BUSCANDO SAÍDA");

    alinhar_ultra(256);
    while (ultra(0) > 180)
    {
        print(2, "Verificando direita");
        mover(300, 300);
        if (ultra(1) > 300)
        {
            print(3, "Encontrada!");
            som("B2", 150);
            alinhar_ultra(224);
            som("C3", 150);
            girar_direita(90);
            alinhar_angulo();
            alcancar_saida();
            return;
        }
    }
    print(3, "Saida a direita não encontrada");
    som("D3", 150);
    som("C3", 150);

    alinhar_ultra(100);
    limpar_console();
    print(1, "BUSCANDO SAÍDA");
    girar_direita(45);
    while (ultra(0) > 103)
    {
        print(2, "Verificando esquerda");
        mover(300, 300);
        if (ultra(2) > 256)
        {
            print(3, "Encontrada!");
            som("B2", 150);
            girar_esquerda(45);
            alinhar_ultra(32);
            som("C3", 150);
            girar_esquerda(45);
            encoder(300, 4);
            girar_esquerda(45);
            alinhar_angulo();
            alcancar_saida();
            return;
        }
    }
    print(3, "Saida a esquerda não encontrada");
    som("D3", 150);
    som("C3", 150);

    girar_direita(45);
    limpar_console();
    print(2, "indo para saida a frente na direita");
    alinhar_angulo();
    alinhar_ultra(32);
    girar_esquerda(45);
    encoder(300, 4);
    girar_esquerda(45);
    alinhar_angulo();
    alcancar_saida();
    return;
}
void achar_saida()
{

    float direcao_inicial = 0; // variavel para a posição inical do robô

    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 10f; // constande de sensibilidade para encontrar triangulo 

    direcao_saida = 0;      //inicia as localizações zeradas 
    direcao_triangulo = 0;

    alinhar_angulo();
    encoder(100, 10); // empurra possiveis bolinhas para frente
    encoder(-250, 10);
    encoder(250, 5);
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    delay(511);
    abaixar_atuador();
    delay(511);
    alinhar_angulo();

    direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate

    while (ultra(0) > 180) // enqunto estiver a mais de 180cm da parede frontal busca por saida ou triangulo
    {
        mover(180, 180);
        ler_ultra();
        if (ultra_direita > 300)  // caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = converter_graus(direcao_inicial + 90); // determina que a saida está a direita
            print(1, "saida na direita encontrada");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = converter_graus(direcao_inicial + 135); // determina que o triangulo está a direita
            print(2, "triangulo encontrado na direita");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }

    alinhar_ultra(105); // move o robô até o ultrasonico frontal registrar 67cm para iniciar verificação do canto esquerdo
    levantar_atuador();
    delay(511);
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    parar();
    delay(64);
    if (luz(4) < 2) // verifica se o triangula esta lá
    {
        direcao_triangulo = converter_graus(direcao_inicial - 45); // determina que o triangulo está a esquerda
        print(2, "triangulo encontrado na esquerda");
        som("D3", 150);
        som("C3", 150);
    }

    if (luz(4) > 2)
    {
        alinhar_angulo();
        girar_direita(45); // vira 45º para efetuar verificação com ultrasonico lateral
        ler_ultra();

        while (ultra_frente > 26) // enqunto estiver a mais de 26cm da parede frontal busca por saida
        {
            mover(200, 200);
            ler_ultra();
            if (ultra_esquerda > 300) // caso o ultrasonico da lateral esquerda veja uma distancia muito grande o robô encontrou a saida
            {
                direcao_saida = converter_graus(direcao_inicial - 90); // determina que a saida está a esquerda
                print(1, "saida na esquerda encontrada");
                som("D3", 300);
                som("C3", 300);
                break;
            }
        }
    }

    objetivo_direita((int)converter_graus(direcao_inicial + 90));
    alinhar_ultra(100);
    mover(200, 200);
    delay(2048);
    if (luz(4) < 2) // verifica se o triangula esta lá
    {
        direcao_triangulo = converter_graus(direcao_inicial + 45); // determina que o triangulo está a direita na frente
        print(2, "triangulo encontrado na frente direita");
        som("D3", 150);
        som("C3", 150);
    }

    ler_ultra();
    while (ultra_frente < 65) // anda para tras procurando saida
    {
        mover(-250, -250);
        ler_ultra();
        if (ultra_esquerda > 300)
        {
            direcao_saida = direcao_inicial; // determina que a saida está na frente a direita
            print(1, "saida na frente direita");
            som("D3", 300);
            som("C3", 300);
            break;
        }
    }

    if (direcao_saida == 0) // se a saida ainda não foi encontrada ela está na ultima posição possivel
    {
        direcao_saida = converter_graus(direcao_inicial + 90); // determina que a saida está na direita
        print(1, "saida na frente direita");
        som("D3", 300);
        som("C3", 300);

    }
    if (direcao_triangulo == 0) // se o triangulo ainda não foi encontrado ele está na ultima possição possivel
    {
        direcao_triangulo = converter_graus(direcao_inicial + 135); // determina que o triangulo a direita
        print(2, "triangulo encontrado na frente direita");
        som("D3", 150);
        som("C3", 150);
    }
}

// Variável de controle para ligar/desligar o debug
bool debug = false;
bool console = true;

// Método principal
void Main()
{
    if (!debug)
    {
        calibrar();
        ultima_correcao = millis();
        abaixar_atuador();
    }
    // Loop principal do programa
    while (!debug)
    {
        while (lugar == "piso")
        {
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_elevada();
            verifica_rampa_resgate();
        }
        limpar_console();
        print(1, "SUBINDO RAMPA");
        led(255, 0, 0);
        som("B2", 500);
        while (lugar == "rampa resgate")
        {
            velocidade = 250;
            seguir_rampa();
            if ((eixo_y() > 355) || (eixo_y() < 5))
            {
                lugar = "resgate";
            }
        }
        limpar_console();
        while (lugar == "resgate")
        {
            achar_saida();
            travar();
            limpar_console();
            while (verde(0) || verde(1) || verde(2) || verde(3))
                mover(200, 200);
            delay(64);
            parar();
            mover(200, 200);
            delay(16);
            parar();
            lugar = "percurso de saida";
        }
        abaixar_atuador();
        delay(700);
        while (lugar == "percurso de saida")
        {
            if (verifica_saida()) { encoder(300, 15); travar(); }
            verifica_obstaculo();
            seguir_linha();
            verifica_calibrar();
            verifica_elevada();
        }
    }

    // Loop para debug
    while (debug)
    {
        mover(200, 200);
        delay(5);
        travar();
    }
}
