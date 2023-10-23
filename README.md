**AISquad, Dual Utility Reasoner et Modular Decision Making**

de Yann Déchaux et Umut Osmanoglu




**I.Contrôles**

ZQSD/WASD : Déplacement de base

Clic gauche : Tir principal 

Clic droit : Tir de soutien des IA

Touches alphanumérique de 1 à 6 : Différentes formations

La molette de la souris (pendant la formation) : change la distance entre joueur et IA.

**II.Formations**

1) Logique principal de l’IA : Il suit le joueur, bloque les tirs ennemis/riposte, et le soigne selon sa vie.

2) Formation tortue : Les IA forment un cercle autour du joueur afin de le protéger\.

3) Formation bouclier : Les IA forment une ligne de front, afin de bloquer les tirs venant de devant le joueur,

4) Formations “surround” : Les IA s'organisent en carré autour du joueur, qui tourne selon son orientation\.

5) Formation “mouse” : Les IA forment un cercle autour de la souris\.

6) Formation “freeze” : Les IA gardent leurs positions actuelles et tirent sur les ennemis/sur la cible de couverture/ à l’endroit où le joueur tire\.



**III.Logique de l’IA**

Pour effectuer leurs choix, nos IA se reposent sur la logique connue sous le nom de “utility reasoning”, plus précisément le “dual utility reasoning” .

Chaque comportement que nous souhaitons implémenter à notre IA est un script, qui hérite de la classe “ Utility Behavior” 

![](001.png)

![](002.png)

Cette classe contient : 

- Une fonction Update Behavior, contenant la logique de l'exécution du comportement.
- Une liste de considérations, qui correspond aux conditions de l'exécution du comportement. 
- Une fonction GetRank et GetWeight, qui, en dépendant de notre liste de considérations, nous envoie un score de priorité basé sur deux variables : le poids, et le rang.



En effet, contrairement au simple utility reasoner qui évalue chaque option et choisit celle ayant la plus grande utilité, le dual utility reasoner attribue deux valeurs d'utilité à chaque option: 

Le rang, utilisé pour diviser les options en catégories, ne sélectionnant que les options de la meilleure catégorie. 

Le poids,évaluant les options dans la même catégorie.

Le poids et le rang de chaque comportement est définie selon sa liste de considérations, mentionnée plus tôt, c’est le principe du “Modular Decision Making” :  la logique de chaque décision est décomposée en une ou plusieurs considérations. Chaque considération examine la situation selon un aspect unique (si le player est proche/loin/ n’a plus de vie) et renvoie une évaluation qui, lorsqu'elle est combinée à celles des autres considérations, guide la décision globale en retournant un rang, un bonus et un multiplicateur.

Et donc, le rang d'un comportement est déterminé par le rang le plus élevé parmi ses considérations, tandis que le poids est calculé en multipliant la somme de ses bonus par le produit de ses multiplicateurs.

Ensuite, dans notre scène, nous avons un AIManager, qui s’occupe de trouver le meilleur comportement à suivre pour chaque IA. Bien sûr, cette logique n’est pas appelée à chaque frame, mais à un temps de réaction humain (0.25 s, soit toutes les 20 frames environ, selon les PC).

La logique généralement utilisée lors du “Dual Utility Reasoning” contient ces 4 étapes : 

-   Éliminez toutes les options avec un poids de 0.
- Déterminez la catégorie de rang la plus élevée et éliminez les options avec un rang inférieur.
- Éliminez les options dont le poids est significativement inférieur à celui de la meilleure option restante.
- Utilisez une sélection aléatoire pondérée sur les options restantes.

Dans notre cas, vu que nos rangs sont majoritairement entre 0 et 1, le poids sera plus souvent utile lorsque le rang sera égale à 1, ou bien lorsque l’on veut écarter un comportement selon une considération spéciale ( vu que les options ayant un poids étant égal à 0 sont automatiquement écarter).


On possède également un Formation Manager, car , lorsqu’il s’agit d’organiser plusieurs IA en une formation spécifique, il est plus simple, propre et économique de centraliser la logique en un code, plutôt que de laisser chaque IA décider de sa position selon celle des autres. 




**Sources :** 

[GameAIPro2_Chapter03_Dual-Utility_Reasoning.pdf](https://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter03_Dual-Utility_Reasoning.pdf)

[Microsoft Word - dill_designpatterns.doc (neu.edu)](https://course.ccs.neu.edu/cs5150f13/readings/dill_designpatterns.pdf)
