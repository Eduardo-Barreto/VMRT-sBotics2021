// Declaração das variáveis principais de todo o projeto, separadas por tipos
byte velocidade_padrao = 185,
        velocidade = 180,
        velocidade_max = 220,
        media_meio = 0,
        direcao_triangulo = 0,
        direcao_saida = 0,
        lugar = 0,
        limite_branco = 40; // 40 ou 55 (beta)

float saida1 = 0,
        saida2 = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0;

int tempo_correcao = 0,
        update_time = 16,
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

short[] angulos_retos = { 0, 90, 180, 270 };

char lado_ajuste = '0';
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

void erro(object texto)
{
    throw new Exception(texto.ToString());
}

string[] rainbow = { "#f90300", "#f89621", "#fce91f", "#42b253", "#2aaae1", "#0047ab", "#9400d3" };

void rainbow_console(string word, string[] colors, int time = 5000)
{

    string word_final = "";
    int colors_index = 0;

    string colorize(char texto, string cor) => $"<color={cor}>{texto}</color>";

    bot.ResetTimer();
    while (bot.Timer() < time)
    {
        colors_index++;

        word_final = "";
        for (byte i = 0; i < word.Length; i++)
        {
            word_final += colorize(word[i], colors[colors_index % colors.Length]);
            bot.TurnLedOn(colors[colors_index % colors.Length]);
            colors_index++;
        }

        bot.Print($"<b><size=60><align=center>{word_final}</align></size></b>\n");
        bot.Wait(200);
    }

}
// Métodos de leitura e outros

int millis() => (int)(bot.Timer());
bool toque() => (bot.Touch(0));
string cor(byte sensor) => bot.ReturnColor(sensor);
int luz(byte sensor) => (int)bot.Lightness(sensor);
int ultra(byte sensor) => (int)bot.Distance(sensor);
float eixo_x() => bot.Compass();
float eixo_y() => bot.Inclination();
float angulo_atuador() => bot.AngleActuator();
float angulo_giro_atuador() => bot.AngleScoop();
bool tem_vitima() => bot.HasVictim();
void delay(int milissegundos) => bot.Wait(milissegundos);

void som(string nota, int tempo) => bot.PlayNote(0, nota, tempo);

Dictionary<string, string> cores = new Dictionary<string, string>(){
    {"preto", "#000000"},
    {"cinza escuro", "#333332"},
    {"cinza claro", "#656565"},
    {"branco", "#fffffe"},
    {"amarelo", "#ffcc02"},
    {"verde", "#009245"},
    {"vermelho", "#ff3232"},
    {"azul", "#28ade2"}
};
void led(byte R, byte G, byte B) => bot.TurnLedOn(R, G, B);
void led(string cor)
{
    if (cor == "desligado")
    {
        bot.TurnLedOff();
        return;
    }
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    bot.TurnLedOn(cor);
}

void console_led(byte linha, object texto, string cor, bool ligar_led = true)
{
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    string texto_aux = texto.ToString();
    texto_aux = texto_aux.Replace("<:", $"<color={cor}>");
    texto_aux = texto_aux.Replace(":>", "</color>");
    print(linha, "<align=center>" + texto_aux.ToString() + "</align>");
    if (!ligar_led)
    {
        bot.TurnLedOff();
        return;
    }
    bot.TurnLedOn(cor);
}


void print(byte linha, object texto) { if (console) bot.Print(linha - 1, "<align=center>" + texto.ToString() + "</align>"); }

void limpar_console() => bot.ClearConsole();
void limpar_linha(byte linha) => print((byte)(linha - 1), " ");

bool tem_linha(byte sensor) => (bot.returnRed(sensor) < 24);

bool vermelho(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 66, media_verde = 16, media_azul = 16;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERMELHO");
}

/* bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    print(1, $"{vermelho} | {verde} | {azul}");
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96) || cor(sensor) == "VERDE");
} */

bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 13, media_verde = 82, media_azul = 4;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERDE");
}

bool preto(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) < media_meio) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) < limite_branco) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    return false;
}

bool branco(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) > media_meio) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) > limite_branco) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    return false;
}

void calibrar()
{
    alinhar_linha(true);
    media_meio = (byte)Math.Round((luz(1) + luz(2)) / 3f);

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

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


string luz_marker(int luz)
{
    string hexStr = Convert.ToString(luz, 16);
    string grayscaleHex = (hexStr.Length < 2) ? ("0" + hexStr) : hexStr;
    string marker = '#' + grayscaleHex + grayscaleHex + grayscaleHex;
    return $"<mark={marker}>--</mark>";
}

void print_luz_marker()
{
    string luz0 = ((luz(0).ToString()).Length < 2) ? $"0{luz(0)}" : luz(0).ToString();
    string luz1 = ((luz(1).ToString()).Length < 2) ? $"0{luz(1)}" : luz(1).ToString();
    string luz2 = ((luz(2).ToString()).Length < 2) ? $"0{luz(2)}" : luz(2).ToString();
    string luz3 = ((luz(3).ToString()).Length < 2) ? $"0{luz(3)}" : luz(3).ToString();
    print(2, $"{luz3} | {luz2} | {luz1} | {luz0}");
    print(3, $"{luz_marker(luz(3))} | {luz_marker(luz(2))} | {luz_marker(luz(1))} | {luz_marker(luz(0))}");
    print(4, "");
}
// Métodos de movimentação e outros

void abrir_atuador() => bot.OpenActuator();
void fechar_atuador() => bot.CloseActuator();
void mover(int esquerda, int direita) => bot.MoveFrontal(direita, esquerda);
void encoder(int velocidade, float rotacoes) => bot.MoveFrontalRotations(velocidade, rotacoes);
void parar(int tempo = 10) { bot.MoveFrontal(0, 0); delay(tempo); }
void travar() { bot.MoveFrontal(0, 0); delay(999999999); }

void mover_tempo(int velocidade, int tempo)
{
    int timeout = bot.Timer() + tempo;
    while (bot.Timer() < timeout)
    {
        mover(velocidade, velocidade);
    }
    parar();
}

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
void objetivo_esquerda(float objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(float objetivo)
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
    led("amarelo");

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

    led("desligado");
}

// Ajusta os sensores na linha preta
void alinhar_linha(bool por_luz = false)
{
    led("amarelo");

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
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(1) < 30 && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(3) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(2) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
    }
    else
    {
        tempo_correcao = millis() + 150;
        while (cor(0) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(1) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(3) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(2) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
    }

    delay(64);
    parar();
    led("desligado");
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bot.ActuatorSpeed(150);
    bot.ActuatorUp(100);
    if (angulo_atuador() >= 0 && angulo_atuador() < 88)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorUp(600);
    }
}

void abaixar_atuador()
{
    if (angulo_atuador() > 5)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorDown(600);
    }
}
bool verifica_saida()
{
    if (lugar == 3)
    {
        if ((vermelho(0)) || (vermelho(1)) || (vermelho(2)) || (vermelho(3)))
        {
            alinhar_angulo();
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 175);
            som("MUDO", 200);

            som("A#2", 112);
            som("A2", 112);
            som("G2", 112);
            som("MUDO", 15);
            som("F2", 300);
            som("MUDO", 150);
            som("F3", 300);
            encoder(300, 15);
            rainbow_console("ARENA FINALIZADA", rainbow);
            travar();
            return true;
        }
        return false;
    }
    // Está saindo da pista (detectou o fim da arena)
    else if (vermelho(1) || vermelho(2))
    {
        console_led(1, "<:Saí da arena...:>", "vermelho");
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
                if (!vermelho(1) && !vermelho(2))
                {
                    delay(200);
                    break;
                }
            }
        }
        alinhar_linha();
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
    console_led(1, $"Seguindo linha: <:{velocidade}:>", "azul", false);
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            alinhar_linha();
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        float objetivo = (lado_ajuste == 'd') ? (converter_graus(eixo_x() + 10)) : (converter_graus(eixo_x() - 10));
        while (!proximo(eixo_x(), objetivo))
        {
            if (lado_ajuste == 'd')
                mover(1000, -1000);
            else
                mover(-1000, 1000);

            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                alinhar_linha(true);
                alinhar_linha();
                ultima_correcao = millis();
                return;
            }
        }
        if (lado_ajuste == 'd')
            girar_esquerda(10);
        else
            girar_direita(10);

        parar();

        // Confirma que está perdido
        console_led(1, "<:Perdi a linha...:>", "vermelho");
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
            if (toque())
            {
                parar();
                break;
            }
        }
        delay(150);
        alinhar_linha(true);
        alinhar_linha();
        velocidade = (byte)(velocidade - 15);
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
        lado_ajuste = 'd';
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
        lado_ajuste = 'e';
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        verifica_curva();
        mover(velocidade, velocidade);
    }
}
bool falso_verde()
{
    /*
    Falso Verde: Verifica se o robô realmente está no verde e não passou reto de uma outra encruzilhada
        Vem da verificação do verde
        Define um tempo máximo de verificação de 200 milissegundos
        Enquanto está nesse tempo:
            Anda para trás
            Se encontrar a cor preta, vai para frente e retorna verdadeiro (era realmente um falso verde)
            Senão, continua a movimentação, retorna falso (falso falso verde = verde verdadeiro), e realiza a curva
    */
    parar();
    int tempo_check_preto = millis() + 200;
    while (millis() < tempo_check_preto)
    {
        mover(-180, -180);
        if (cor(0) == "PRETO" || cor(3) == "PRETO")
        {
            mover_tempo(300, 288);
            return true;
        }
    }
    mover(200, 200);
    delay(196);
    parar();
    return false;
}

bool beco()
{
    /*
    Beco: Verifica se o robô está no Beco sem saída (verde dos dois lados da encruzilhada)
        Para o robô por tempo suficiente para atualizar a leitura dos sensores
        Se detectar verde dos dois lados:
            Ajusta na linha e para novamente por tempo suficiente para atualizar a leitura dos sensores
            Se novamente identificar verde dos dois lados
                Verifica se é realmente uma encruzilhada (falso_verde())
                Confirmado o beco, acende o led verde e escreve no console
                Indica pelo som que caiu na condição correta
                Vai para frente e faz uma curva de 170 graus para a direita
                Gira até encontrar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Para, lê as cores e verifica se está na condição do beco
    parar(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        alinhar_linha();
        delay(64);
        ler_cor();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição
            console_led(1, "<:<b>BECO SEM SAÍDA</b>:>", "verde");
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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

    /*
    Verifica Verde: Verifica se o robô está em uma encruzilhada com verde
        Atualiza as leituras dos sensores
        Se encontrar verde em algum dos sensores da direita
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à direita
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar verde em algum dos sensores da esquerda
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à esquerda
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha e verifica novamente
        alinhar_linha();
        delay(64);
        alinhar_linha();
        ler_cor();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Direita", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 447);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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

    // Verifica os sensores da esquerda
    else if (verde2 || verde3)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha e verifica novamente
        alinhar_linha();
        delay(64);
        alinhar_linha();
        ler_cor();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Esquerda", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 447);
            girar_esquerda(30);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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
    /*
    Verifica Curva: Verifica se o robô está em alguma curva de 90° no preto
        Atualiza as leituras dos sensores
        Verifica se está no verde ou no fim da arena
        Se encontrar preto no sensor da direita
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
            Atualiza a leitura dos sensores
            Se estiver no vermelho (fim da arena), retorna falso (não é curva)
            Se encontrar preto no sensor da esquerda (encruz reta)
                Vai para frente e retorna falso (não é curva)
            Verifica novamente pela saída da arena (vermelho) e verde
            Vai para trás e verifica novamente pelo verde
            Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
            Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
            Com a curva totalmente confirmada, continua girando até achar a linha
            Se passar de 115 graus, assume que é o ladrilho de Curva C com GAP
                Se alinha atrás e por graus
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
            Se alinha na linha
            Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar preto no sensor da esquerda
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
                Atualiza a leitura dos sensores
                Se estiver no vermelho (fim da arena), retorna falso (não é curva)
                Se encontrar preto no sensor da direita (encruz reta)
                    Vai para frente e retorna falso (não é curva)
                Verifica novamente pela saída da arena (vermelho) e verde
                Vai para trás e verifica novamente pelo verde
                Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
                Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
                Com a curva totalmente confirmada, continua girando até achar a linha
                Se passar de 115 graus, assume que é o ladrilho de Curva C com GAP
                    Se alinha atrás e por graus
                    Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
                Se alinha na linha
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza leituras de cores, verifica se está no verde e depois no vermelho
    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        parar(64);
        print_luz_marker();
        ler_cor();
        if (vermelho(0)) { return false; }
        if (preto_curva_esq)
        {
            mover_tempo(300, 288);
            return false;
        }
        if (verifica_saida()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        if (verifica_verde()) { return true; }
        mover_tempo(-300, 143);
        if (verifica_verde()) { return true; }
        // Feedbacks visuais e sonoross de que entrou na condição da curva
        console_led(1, "<b>CURVA PRETO</b> - Direita", "preto");
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        mover_tempo(300, 351);
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
        while (!tem_linha(1) || vermelho(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                /* Se chegar ao ângulo máximo, é uma curva com um gap no final
                Se alinha e arruma a curva de 90 somente com a referência de graus*/
                mover_tempo(-300, 239);
                mover(-1000, 1000);
                delay(650);
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(1000, -1000);
        }
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        alinhar_linha();
        encoder(-300, 2);
        alinhar_linha(true);
        alinhar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar(64);
        print_luz_marker();
        ler_cor();
        if (vermelho(3)) { return false; }
        if (preto_curva_dir)
        {
            mover_tempo(300, 288);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return true; }
        mover_tempo(-300, 143);
        if (verifica_verde()) { return true; }
        console_led(1, "<b>CURVA PRETO</b> - Esquerda", "preto");
        som("C3", 100);
        mover_tempo(300, 351);
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
        while (!tem_linha(2) || vermelho(2))
        {
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 239);
                mover(1000, -1000);
                delay(650);
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(-1000, 1000);
        }
        delay(200);
        alinhar_linha();
        encoder(-300, 2);
        alinhar_linha(true);
        alinhar_linha(true);
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
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        alinhar_angulo();
        if (ultra(0) < 12)
            mover_tempo(-200, 159);
        if (ultra(0) < 12)
            mover_tempo(-100, 50);
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
        mover_tempo(300, 575);
        som("E3", 32);
        girar_esquerda(45);
        som("E3", 32);
        int timeout_obstaculo = millis() + 591;
        while (millis() < timeout_obstaculo)
        {
            if (preto(0) || preto(1))
            {
                break;
            }
            mover(200, 200);
        }
        parar();
        som("D3", 32);
        mover_tempo(300, 399);
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
bool verifica_gangorra()
{
    /*
    Verifica gangorra: Verifica se o robô está numa gangorra
        Vindo do verifica_rampa, se estiver num ângulo próximo a 0, com 5 de tolerancia
            Alinha no ângulo ortogonal mais próximo
            Escreve no console que está na gangorra e acende o led vermelho
            Vai um pouquinho pra trás e espera 600 milissegundos
            Alinha no ângulo ortogonal mais próximo novamente
            retorna verdadeiro
    */

    if (eixo_y() > 350 || eixo_y() < 5)
    {
        alinhar_angulo();
        parar();
        console_led(1, "<:GANGORRA:>", "vermelho");
        encoder(-300, 2);
        delay(600);
        alinhar_angulo();
        return true;
    }
    return false;
}

bool verifica_rampa()
{
    /* 
    Verifica rampa: Verifica se o robô está numa rampa
        Quando a inclinação for próxima de 350
            Levanta o atuador
            Define um tempo para chegar ao topo da rampa
            Inicia a subida
                Segue linha
                Verifica se é uma gangorra
            Abaixa o atuador e retorna

    */
    if (millis() < update_rampa)
        return false;

    if (proximo(eixo_y(), 350))
    {
        parar();
        levantar_atuador();
        int tempo_subir = millis() + 2300;
        bool flag_subiu = false;
        int tempo_check_gangorra = millis() + 400;
        while (millis() < tempo_subir)
        {
            if (millis() > tempo_check_gangorra && proximo(eixo_y(), 340))
            {
                flag_subiu = true;
            }
            if (flag_subiu && verifica_gangorra()) { break; }
            ultima_correcao = millis();
            seguir_linha();
            if (lugar != 3 && verifica_rampa_resgate())
                return true;
        }
        parar();
        if (eixo_y() < 10 || eixo_y() > 40)
        {
            int timeout = millis() + 400;
            while (eixo_y() < 350 || eixo_y() > 5)
            {
                ultima_correcao = millis();
                seguir_linha();
                if (verifica_obstaculo(false))
                    break;
                if (millis() > timeout)
                    break;

            }
        }
        parar();
        abaixar_atuador();
        update_rampa = millis() + 2000;
        return true;
    }
    return false;

}

bool verifica_rampa_resgate()
{
    /*
    Verifica rampa resgate: Verifica se o robô está na rampa do resgate
        Se o eixo y (inclinação) estiver próximo de 340 com uma sensibilidade de 10
        e os dois ultrassônicos do lado estiverem tampados (com parede)
            Define o lugar global como a rampa do resgate e retorna
    */

    if ((proximo(eixo_y(), 340, 10)) && (ultra(1) < 40 && ultra(2) < 40))
    {
        lugar = 1;
        return true;
    }
    return false;
}
// metodos de movimentação para a area de resgate
//;
void alinhar_ultra(int distancia)
{
    if (ultra(0) > distancia)
    {
        while (ultra(0) > distancia + distancia / 6)
        {
            mover(300, 300);
        }
        while (ultra(0) > distancia + distancia / 5)
        {
            mover(200, 200);
        }
        while (ultra(0) > distancia)
        {
            mover(100, 100);
        }
        while (ultra(0) < distancia)
        {
            mover(-75, -75);
        }
    }
    else
    {
        while (ultra(0) < distancia - distancia / 6)
        {
            mover(-300, -300);
        }
        while (ultra(0) < distancia - distancia / 5)
        {
            mover(-200, -200);
        }
        while (ultra(0) < distancia)
        {
            mover(-100, -100);
        }
        while (ultra(0) > distancia)
        {
            mover(75, 75);
        }
    }
    parar();
}

void entregar_vitima()
{
    abrir_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima())
    {
        if (millis() > timeout_vitima)
        {
            levantar_atuador();
            fechar_atuador();
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    levantar_atuador();
    fechar_atuador();
}

void totozinho(byte vezes = 1)
{ // empurra possiveis bolinhas para frente
    for (byte i = 0; i < vezes; i++)
    {
        encoder(250, 10);
        encoder(-300, 10);
    }
    parar();
}

void preparar_atuador(bool apenas_sem_vitima = false)
{
    if (apenas_sem_vitima)
    {
        if (!tem_vitima())
        {
            totozinho();
            alinhar_angulo();
            abaixar_atuador();
            abrir_atuador();
        }
    }
    else
    {
        totozinho();
        alinhar_angulo();
        abaixar_atuador();
        abrir_atuador();
    }
}
void seguir_rampa()
{
    for (; ; )
    {
        ler_cor();

        if ((eixo_y() > 355) || (eixo_y() < 5))
        {
            lugar = 2;
            return;
        }

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
            mover(300, 300);
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
            mover(300, 300);
            delay(5);
        }

        // Se está certo na linha só vai para frente com a velocidade atual
        else
        {
            mover(300, 300);
        }
    }
}
void alcancar_saida()
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
    bot.TurnLedOff();

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
                sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo 

    direcao_saida = 0;      //inicia as localizações zeradas 
    direcao_triangulo = 0;

    alinhar_angulo();
    totozinho();
    alinhar_angulo();
    preparar_atuador();
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    alinhar_angulo();

    direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate
    ler_ultra();
    while (ultra_frente > 180) // enqunto estiver a mais de 180cm da parede frontal busca por saida ou triangulo
    {
        ler_ultra();
        mover(180, 180);
        if (ultra_direita > 300)  // caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = 3; // determina que a saida está a direita
            print(1, "SAÍDA DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = 3; // determina que o triangulo está a direita
            print(2, "TRIÂNGULO DIREITA");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }
    mover(300, 300);
    delay(1500);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(105); // move o robô até o ultrasonico frontal registrar 67cm para iniciar verificação do canto esquerdo
    delay(511);
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    parar(64);
    alinhar_angulo();
    if (luz(4) < 2) // verifica se o triangula esta lá
    {
        direcao_triangulo = 1; // determina que o triangulo está a esquerda
        print(2, "TRIÂNGULO ESQUERDA");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            girar_esquerda(90);
            entregar_vitima();
            girar_direita(90);
            alinhar_ultra(26);
        }
        else
        {
            girar_direita(45);
            alinhar_ultra(26);
        }
        if (direcao_saida != 0) return;

    }

    else
    {
        alinhar_angulo();
        girar_direita(45); // vira 45º para efetuar verificação com ultrasonico lateral
        ler_ultra();

        while (ultra_frente > 26) // enqunto estiver a mais de 26cm da parede frontal busca por saida
        {
            ler_ultra();
            mover(200, 200);
            if (ultra_esquerda > 300 && direcao_saida == 0) // caso o ultrasonico da lateral esquerda veja uma distancia muito grande o robô encontrou a saida
            {
                direcao_saida = 1; // determina que a saida está a esquerda
                print(1, "SAÍDA ESQUERDA");
                som("D3", 300);
                som("C3", 300);
            }
        }
    }

    objetivo_direita(converter_graus(direcao_inicial + 90));
    preparar_atuador(true);
    mover(300, 300);
    delay(650);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    alinhar_angulo();
    delay(64);

    if (luz(4) < 2 && direcao_triangulo == 0)
    {
        direcao_triangulo = 2; // determina que o triangulo está a direita na frente
        print(2, "TRIANGULO FRONTAL DIREITA");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            girar_esquerda(90);
            entregar_vitima();
        }
    }

    ler_ultra();
    while (ultra_frente < 65) // anda para tras procurando saida
    {
        mover(-250, -250);
        ler_ultra();
        if (ultra_esquerda > 300)
        {
            direcao_saida = 2; // determina que a saida está na frente a direita
            print(1, "SAIDA FRONTAL DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
    }

    if (direcao_saida == 0) // se a saida ainda não foi encontrada ela está na ultima posição possivel
    {
        direcao_saida = 3; // determina que a saida está a direita
        print(1, "SAÍDA DIREITA");
        som("D3", 300);
        som("C3", 300);

    }
    if (direcao_triangulo == 0) // se o triangulo ainda não foi encontrado ele está na ultima possição possivel
    {
        direcao_triangulo = 3; // determina que o triangulo está a direita
        print(2, "TRIÂNGULO DIREITA");
        som("D3", 150);
        som("C3", 150);
    }

    if (direcao_triangulo == 1 || direcao_triangulo == 2)
    {
        mover(-300, -300);
        delay(300);
        girar_esquerda(5);
        objetivo_direita(converter_graus(direcao_inicial + 90));
        alinhar_ultra(124);
        alinhar_angulo();
        alinhar_ultra(124);
        girar_direita(90);
        alinhar_angulo();
        mover(-300, -300);
        delay(500);
        alinhar_angulo();
        int timeout = millis() + 400;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
    }
    else
    {
        objetivo_direita(converter_graus(direcao_inicial + 180));
        alinhar_angulo();
        alinhar_ultra(124);
        alinhar_angulo();
        alinhar_ultra(124);
        girar_direita(90);
        alinhar_angulo();
        mover(-300, -300);
        delay(750);
        alinhar_angulo();
        int timeout = millis() + 300;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
    }
}

// Variável de controle para ligar/desligar o debug
bool debug = false;
bool console = true;

// Método principal
void Main()
{
    if (debug)
    {
        print(2, "🟪");
        delay(99999);
    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        abaixar_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_rampa_resgate();
        }
        while (lugar == 1)
        {
            limpar_console();
            levantar_atuador();
            console_led(1, "<size=\"60\"><:SUBINDO A RAMPA!:></size>", "azul");
            som("B2", 500);
            seguir_rampa();
        }
        console_led(3, "<:Local atual: RESGATE:>", "cinza claro", false);
        while (lugar == 2)
        {
            sair();
            limpar_console();
            while (verde(0) || verde(1) || verde(2) || verde(3))
                mover(200, 200);
            delay(150);
            delay(64);
            parar();
            mover(200, 200);
            delay(16);
            parar();
            abaixar_atuador();
            delay(700);
            lugar = 3;
        }
        console_led(3, "<:Local atual: PERCURSO DE SAÍDA:>", "cinza claro", false);
        while (lugar == 3)
        {
            verifica_saida();
            verifica_obstaculo();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
        }
    }
}
