# Mori-Project

Mori Project

Unity로 제작한 2D 스토리형 게임 프로젝트입니다.
플레이어는 점술사가 되어 손님과 대화를 진행하고, 타로 카드를 활용한 미니게임을 통해 이야기의 흐름을 이어갑니다.

프로젝트 소개

Mori는 대화 선택지와 타로 카드 미니게임을 결합한 2D 게임입니다.
Ink 기반 대화 시스템을 사용하여 스토리를 진행하며, 손님의 질문에 따라 선택지를 고르고 카드 게임 결과에 따라 다음 대화가 이어지는 구조로 제작했습니다.

주요 기능
Ink 기반 대화 시스템
선택지에 따른 대화 진행
손님 순서 및 하루 진행 관리
타로 카드 매칭 미니게임
제한 시간, HP, 게임 성공/실패 처리
상점, 설정, 맵, 엔딩 등 기본 UI 구성
DOTween을 활용한 카드 애니메이션 및 UI 연출
사용 기술
Unity 6000.0.40f1
C#
TextMeshPro
Ink
DOTween
Universal Render Pipeline 2D
실행 방법
Unity Hub에서 Mori 폴더를 열어줍니다.
Unity 버전은 6000.0.40f1을 권장합니다.
Assets/Scenes/Junho/Start/Start Scene.unity 씬을 실행합니다.
Play 버튼을 눌러 게임을 시작합니다.
프로젝트 구조
Mori
├── Assets
│   ├── Scenes
│   │   ├── Junho      # 시작 화면 및 대화 시스템
│   │   ├── Reahyeon   # UI, 상점, 설정 화면
│   │   └── Youngjin   # 카드 매칭 미니게임
│   ├── Ink            # Ink 대화 시스템
│   └── Plugins        # DOTween 등 외부 플러그인
개발 내용 요약

이 프로젝트에서는 Unity의 씬 전환, UI 패널 관리, 대화 시스템, 카드 게임 로직을 구현했습니다.
특히 대화 결과와 미니게임 결과가 연결되도록 구성하여 단순한 카드 게임이 아니라 스토리 진행과 연결되는 게임 흐름을 만드는 데 집중했습니다.
