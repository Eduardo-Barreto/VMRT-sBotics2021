// Funções de cálculo

Func<float, float, float, float, float, int> map = delegate(float val, float minimo, float maximo, float minimoSaida, float maximoSaida) {
	// "mapeia" ou reescala um val(val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
	return (int)( (((val - minimo) * (maximoSaida - minimoSaida)) / (maximo - minimo)) + minimoSaida);
};

Func<float, float> converter_graus= (graus) => {
	// converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
	float graus_convertidos = graus;
	graus_convertidos = (graus_convertidos < 0) ? 360 + graus_convertidos : graus_convertidos;
	graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
	graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
	return graus_convertidos;
};

// Verifica se um val(val) esta entre um val mínimo(minimo) e máximo(maximo)
Func <float, float, float, bool> intervalo = (val, minimo, maximo) => (val > minimo && val < maximo);