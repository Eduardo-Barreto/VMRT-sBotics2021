// Funções de cálculo

Func<float, float, float, float> limitar = delegate(float amt,float low,float high){
   return ((amt)<(low)?(low):((amt)>(high)?(high):(amt)));
};

Func<float, float, float, float, float, int> map = delegate(float value, float min, float max, float minTo, float maxTo) {
  return (int)( (((value - min) * (maxTo - minTo)) / (max - min)) + minTo);
};

Func<float, float> converter_graus= (graus) => {
	float graus_convertidos = graus;
	graus_convertidos = (graus_convertidos < 0) ? 360 + graus_convertidos : graus_convertidos;
	graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
	graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
	return graus_convertidos;
};

Func <float, float, float, bool> intervalo = (val, minimo, maximo) => (val > minimo && val < maximo);
