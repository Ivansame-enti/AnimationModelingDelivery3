# AnimationModelingDelivery3
--------------------------------------------------------------------------

Descripción del grupo: Estudiantes de CDI en 3º de ENTI-UB.

Pablo Perpiñan Cutillas (pablo.perpinan@enti.cat)

Josep Romera Andreu (josep.romera@enti.cat)

Ivan Sales Méndez (ivan.sales@enti.cat)

--------------------------------------------------------------------------

Controles:

    R: Reinicia la escena,
    I: Activar/Desactivar guiar,
    X/Z: Ajustar magus,
    Espacio: Mantener para cargar el disparo y dejarlo ir para chutar
--------------------------------------------------------------------------

--------------------------------------------------------------------------

Scripts:

    Ejercicio 1:
        1.1.- IK_tentacles y RestartController
        1.2.- MovingBall
        1.3.- IK_Scorpion, MovingBall (En el objeto slider_force estan los valores que puede tomar)
        1.4.- ResetController (en el objeto Restart de la escena)

    Ejercicio 2:
        2.1.- MagnusSliderController (En el objeto slider_force estan los valores que puede tomar), IK_Scorpion
        2.2.- MovingBall (Funcion EulerStep)
        2.3.- MovingBall (Se han agregado las flechas y particulas a la escena, la previsualizacion del magnus effect no la acaba de hacer del todo bien en algunos casos)
        2.4.- MovingBall (Se han agregado las flechas y particulas a la escena, la previsualizacion del magnus effect no la acaba de hacer del todo bien en algunos casos)

    Ejercicio 3:
	    3.1.- IK_Scorpion (El escorpion tiene unos gameobjects en cada pata situados a una cierta altura. Estos tienen un raycast que miran hacia abajo y detectan si hay suelo o obstaculos. Las patas pisan donde el raycast impacte).
	    3.2.- IK_Scorpion (Se han añadido obstaculos. Estos han de tener el tag Obstacles y el terreno debe tener el tag Suelo).
	    3.3.- IK_Scorpion - (Funcion updateLegPos. Un lerp se realiza en un eje Y, y donde finaliza ese, comienza otro lerp que finaliza donde se situa _future_base).
	    3.4.- IK_Scorpion - (Al inicio del update, realizo el promedio de la posicion de las patas en el eje Y, esto se le aplica al cuerpo cambiando su eje Y en funcion de la posicion de las patas.Esto se aplica en un Lerp).
        
    Ejercicio 4:
        4.1.- IK_Scorpion
		4.2.- IK_Scorpion (Funciones de gradientDescent y AngleDiff)

     Ejercicio 5:
        5.1.- IK_tentacles
        5.3.- IK_tentacles(No acababa de quedar del todo bien, por eso esta comentado, pero creemos que la fomra de realizarlo era algo parecido a lo que hemos hecho(linea 397 de IK_tentacles))

--------------------------------------------------------------------------

Explicaciones:

    1.5.- Se ha utilizado Euler Steps para calcular la trayectoria de la pelota hacia el target. En este, se suman la fuerza que recibe del golpe multiplicada por la dirección normalizada, más la aceleración (la cual es de -1 de gravedad (para que esta no afecte demasiado y acabe llegando más o menos al target indicado, y la masa de la pelota que es de 1) multiplicada por el tiempo.
	
    2.5.- Para calcular la RotationVelocity utilizamos esta linea: Vector3 rotationVelocity = Vector3.Cross(movementEulerSpeed, radiusVector) / (ballRadius * ballRadius); Donde vemos que hacemos un cross product entre la suma de fuerzas acumuladas por Euler y el radiusVector(que es el vector que va del centro de la pelota hacia el hit point), una vez tenemos esto lo dividimos entre el Radio de la pelota al cuadrado. ANTOACIÓN: aunque a variable se llame movementEulerSpeed, no es una velocidad, es la suma de la fuerza acumulada por Euler, por lo que es una fuerza.

    2.6.- ForcesExplication.- Nuestra bola se mueve utilizando Euler y haciendo sumatorio de fuerzas entre 3 fuerzas distintas: la acumulación de fuerza de Euler, El magnus Effect y la gravedad.
                    --> La fuerza propia del disparo: calculamos el vector de la pelota al target y aplicamos Euler
                    -->Magnus Effect: Para calcular el Magnus Effect utilizamos la formula: Fm = S(ω x v), donde S es el coficiente de magnus(Magnus_Slider), y el cross product se hace entre RotationVelocity y el vector que va del target a la pelota.
                    Decidimos usar esta formula de todas las que existen, ya que las otras utilizaban factores tales como el fregamento con el aire y similar, variables que nosotros no teniamos, tambien cabe recalcar que de entre todas, esta es la mas simple.

                    -->La fuerza de gravedad la calculamos multiplicando gravedad por masa.

                    Una vez calculadas las 3 fuerzas, utilizamos estas lineas:
                       movementEulerSpeed = movementEulerSpeed + magnusForce + _acceleration Time.deltaTime;
                       transform.position = (transform.position + movementEulerSpeed * Time.deltaTime);

                    En la primera linea hacemos un sumatorio de fuerzas.
                    En la segunda linea aplicamos el transform.position a la pelota.

	4.3.- Para que la cola se ajuste a los parámetros del juego, se recogen los valores de los sliders de fuerza y magnus desde el script IK_Scorpion. Para la fuerza lo que se hace simplemente es cambiar la variable de_learningRate (variable que indica la velocidad de movimiento dependiendo del gradient) por el valor del slider (valores del x al 40) al momento de dejar de pulsar el Espacio. Para el magnus, recogemos el valor del slider (valores del -1 al 1) al momento de estar lo suficientemente cerca de la pelota, para después, a la hora de calcular el gradient, usar este valor sumado a la posición x de la pelota (el centro de esta) para cambiar la posición del target un poco a la izquierda o derecha respectivamente.
	La nueva función de error añadida al gradient descent llamada AngleDiff obtiene la diferencia de ángulos entre el endEffector de la cola y el de la pelota, para que de esta manera se busque que la cola impacte con la misma rotación que tiene la pelota. Además este valor necesita ser absoluto, pues al tratarse de minimizar el valor, este debe ser mayor a 0 para no dar problemas. Ademas se han asignado pesos para cada una de estas funciones para darles mas o menos prioridad.
    1.5.- Se ha utilizado Euler Steps para calcular la trayectoria de la pelota hacia el target. En este, se suman la fuerza que recibe del golpe multiplicada por la dirección normalizada hacia el target, esto sumado a la aceleración (la cual es de -9.81 de gravedad (para que esta no afecte demasiado y acabe llegando más o menos al target indicado, y la masa de la pelota que es de 1) multiplicada por el tiempo.

--------------------------------------------------------------------------