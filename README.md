🎹 JEU MUSICAL (OpenCV + Unity)
Ce projet est un prototype de jeu de rythme inclusif développé sous Unity utilisant la bibliothèque OpenCV (OpenCvSharp). 
L'objectif est de permettre à des personnes en situation de handicap moteur de jouer à un jeu de musique en utilisant uniquement les mouvements de la tête, tout en offrant un mode classique via les mouvements des mains.


📝 Description du Projet
Le jeu consiste à valider des notes musicales (modèles 3D) qui défilent à l'écran en effectuant des actions détectées par une webcam :

Mode Standard : Détection de mouvement dans des zones spécifiques (mains).

Mode Handicap : Détection de l'angle d'inclinaison des yeux (tête).

Le projet intègre un système de score, un chronomètre de 60 secondes, un menu complet avec options de sensibilité et de volume, ainsi qu'une sauvegarde du meilleur score.


🚀 Comment l'utiliser ?
1. Prérequis
Unity 2021.3 ou plus récent.

Une webcam fonctionnelle.

La bibliothèque OpenCvSharp installée dans le projet.

Les fichiers de cascade Haar (haarcascade_frontalface_default.xml et haarcascade_eye.xml) placés dans le dossier Assets/StreamingAssets.

2. Lancement
Ouvrez la scène MenuPrincipal.

Cliquez sur Play.

Passez par le bouton Options pour régler le volume et la sensibilité de la caméra.

Cliquez sur Jouer et choisissez votre profil (Normal ou Handicapé).

Après le compte à rebours de 3 secondes, inclinez la tête ou bougez les mains pour frapper les notes au moment où elles traversent les zones cibles en haut de l'écran.

🛠 Difficultés Rencontrées
Gestion de la WebCam : L'un des plus gros défis a été la gestion du cycle de vie de la WebCamTexture. Lors du rechargement de la scène (bouton Rejouer), la caméra restait parfois bloquée ou affichait un écran noir, 
ce qui a nécessité une libération rigoureuse des ressources mémoire (Dispose) et l'arrêt forcé de la caméra (Stop) à chaque changement de scène.

Changement de concept (Eye Blink vs Eye Slope) : Au départ, nous souhaitions utiliser le clignement des yeux pour valider les notes. Cependant, après plusieurs tests, 
nous avons abandonné cette idée car la détection du clignement via OpenCV est très instable (dépend trop de l'éclairage et de la résolution de la webcam). 
Nous avons pivoté vers l'inclinaison de la tête (angle des yeux), une méthode beaucoup plus robuste, fluide et moins fatigante pour l'utilisateur.

Conflits de types : La gestion des collisions entre les types Rect de Unity et Rect de OpenCV a causé de nombreuses erreurs de compilation au début du développement.


📈 Améliorations Possibles
Bien que le prototype soit fonctionnel, plusieurs axes d'amélioration sont envisageables :

Calibration Automatique : Ajouter une étape de calibration au début pour s'adapter automatiquement à la position de repos de l'utilisateur.

Analyse de Fréquence : Synchroniser automatiquement l'apparition des notes avec le rythme réel du fichier audio (BPM) au lieu d'un intervalle fixe.

Feedbacks Haptiques : Pour les utilisateurs valides, ajouter des vibrations de manette, et pour tous, des effets de particules plus poussés lors des "Hits" parfaits.

Intelligence Artificielle : Utiliser des modèles de Deep Learning (comme MediaPipe) pour une détection faciale encore plus précise dans des conditions de faible luminosité.


👥 Auteur
Projet réalisé dans le cadre d'un prototype de jeu vidéo inclusif.
Merveille NEDUMLUBA-ANG
