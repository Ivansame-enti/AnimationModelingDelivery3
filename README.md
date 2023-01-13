# AnimationModelingDelivery3
--------------------------------------------------------------------------

Descripción del grupo: Estudiantes de CDI en 3º de ENTI-UB.

Pablo Perpiñan Cutillas (pablo.perpinan@enti.cat)

Josep Romera Andreu (josep.romera@enti.cat)

Ivan Sales Méndez (ivan.sales@enti.cat)

--------------------------------------------------------------------------

Controles:
    R: Reinicia la escena
    
--------------------------------------------------------------------------

--------------------------------------------------------------------------

Scripts:
    Ejercicio 1:
        1.1.- IK_tentacles y RestartController
        1.2.- MovingBall
        1.3.- IK_Scorpion, MovingBall (En el objeto slider_force estan los valores que puede tomar)
        1.4.- ResetController (en el objeto Restart de la escena)

    Ejercicio 2:
        2.1.- MagnusSliderController, IK_Scorpion

    Ejercicio 4:
        4.1.- IK_Scorpion
		4.2.- IK_Scorpion

     Ejercicio 5:
        5.1.- IK_tentacles
        5.3.- IK_tentacles(No acababa de quedar del todo bien, por eso esta comentado, pero creemos que la fomra de realizarlo era algo parecido a lo que hemos hecho(linea 397 de IK_tentacles))

--------------------------------------------------------------------------

Explicaciones:
    1.5.- Se ha utilizado Euler Steps para calcular la trayectoria de la pelota hacia el target. En este, se suman la fuerza que recibe del golpe multiplicada por la dirección normalizada, más la aceleración (la cual es de -1 de gravedad (para que esta no afecte demasiado y acabe llegando más o menos al target indicado, y la masa de la pelota que es de 1) multiplicada por el tiempo.
	
	4.3.- Para que la cola se ajuste a los parámetros del juego, se recogen los valores de los sliders de fuerza y magnus desde el script IK_Scorpion. Para la fuerza lo que se hace simplemente es cambiar la variable de_learningRate (variable que indica la velocidad de movimiento dependiendo del gradient) por el valor del slider (valores del x al 40) al momento de dejar de pulsar el Espacio. Para el magnus, recogemos el valor del slider (valores del -1 al 1) al momento de estar lo suficientemente cerca de la pelota, para después, a la hora de calcular el gradient, usar este valor sumado a la posición x de la pelota (el centro de esta) para cambiar la posición del target un poco a la izquierda o derecha respectivamente.
	La nueva función de error añadida al gradient descent llamada AngleDiff obtiene la diferencia de ángulos entre el endEffector de la cola y el de la pelota, para que de esta manera se busque que la cola impacte con la misma rotación que tiene la pelota. Además este valor necesita ser absoluto, pues al tratarse de minimizar el valor, este debe ser mayor a 0 para no dar problemas. Ademas se han asignado pesos para cada una de estas funciones para darles mas o menos prioridad.

--------------------------------------------------------------------------