# SOKOBANT

El sokoban es un juego donde se controla un personaje el cual puede empujar cajas en un *grid* para moverlas a posiciones concretas.

## Arquitectura

- Durante el juego, hay varios elementos que gestionar:
  - Jugador: Un personaje que se mueve en un *grid* 2D. Este recibe input del jugador para desplazarse. EL número de pasos de guarda a modo de puntuación.
  - Caja: Elemento principal del juego. Cuando el jugador intenta moverse contra una caja, esta compruena que la casilla adyacente a la caja en la dirección del empuje esta libre, en caso de ser así, la caja y el personaje pueden desplazarse. Tiene que alcanzar una de las varias metas del nivel.
  - Meta: Son posiciones fijas repartidas por el *grid*. Para superar un nivel, todas las metas deberán estar ocupadas por una caja.
  - Nivel: Es el conjunto de posiciones de jugador, cajas, metas, paredes y otros elementos que forman un escenario.

- Teniendo estos elementos, se pueden sacar diferentes funcionalidades:
  - Movimiento: Tanto las cajas como el player se mueven.
  - Empuje: Cuando el player empuja una caja.
  - Detección de cajas en metas: Detectar si una caja ha entrado o salido de una meta.
  - Comprobación de victoria: Comprobar si todas las metas tienen caja.
  - Cargado de niveles: Poder guardar los niveles de forma que permita su facil edición y carga en el motor.
  - Interfaz: Muestra la selección de niveles, y durante el juego el número de pasos.

- ¿Que gestiona cada cosa?
  - PlayerController:
    - Recepción de input 
      - Desplazamiento, deshacer movimientos, reinicio de nivel, avance de nivel y salida al menú principal
      - Mediante el sistema de *input actions* de Unity, se reciben los eventos y se actua sobre ellos
    - Movimiento del player
      - Una vez recibido el input, se comprueba si el movimiento buscado es valido. Para ello se hace uso de un *raycast* el cual detecta que elemento se encuentra en la casilla adyacente. 
        - Nada: Se realiza el movimiento.
        - Pared: Movimiento no se lleva a cabo. 
        - Caja: La caja comprueba si puede ser empujada, de ser así, se empuja.
      - El movimiento se hace mediante una corrutina, para obtener un movimiento fluido del player sin necesidad de tener que realizar estos calculos en el método Update()
  - BoxComponent:
    - Este componente hereda de la interfaz IPushable, de la cual heredan todos los objetos que puedan ser empujados por el jugador.
    - Cuando el jugador pregunta si puede ser empujada, realiza una comprobación similar a la del player, haciendo uso de un *raycast*.
    - Su movimiento se hace también con una corrutina.
  - FLagComponent:
    - Este componente gestiona la funcionalidad de las metas. Las metas tienen una colisión de tipo *trigger*. Cuando un objeto entra en contacto con esta colisión, esta meta comprueba si es de tipo IPushable. En caso de ser así, lanza un aviso de que esta meta esta ocupada. Más adelante se profundizará en este sistema de avisos. 
    - Cuando cualquier caja se mueve, la meta comprueba si la que se ha movido es la que estaba colocada en la meta. En caso de ser así, la meta pasa a estar vacia y manda un aviso.
  - GameManager:
    - El GameNanager gestiona el estado actual del nivel, y hace de intermediario entre varios sistemas.
    - Recuento de pasos:
      - Cuando el player avanza, el manager gestiona el número de pasos totales, y manda un aviso para actualizar este dato en la interfaz.
    - Gestión de turnos:
      - Cuando el player se mueve, lanza un evento con la referencia al player, la dirección en la que se mueve y la referencia a una caja (en caso de haber movido alguna).
      - Esta lista se gestiona mediante el método LIFO (Last In Firt Out), tratandola así como una cola. 
      - Cuando se pulsa la tecla de retroceso (z), el player manda un evento el cual es recibido por el Manager. Si hay elementos en la cola, se extrae uno y se comprueba si el player y caja pueden realizar el movimiento inverso al descrito. En caso de ser así, el elemento se elimina de la lista y el player y caja retroceden turno. 
      - Al retroceder turno es necesario actualizar el número de pasos dados.
    - Gestión de niveles:
      - TODO
  - LevelManager
  - LevelLoader
    - Sistema de capas
  
- Comunicación entre elementos

- Cajas de colores