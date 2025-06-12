# 🎮 Unity 뱀서라이크 프로젝트

> **프로젝트 개요는 추후 추가 예정입니다!**

---

## ⚠️ 작업 전 필독

- **Unity 버전**: `6.1.7f1`
- **작업 전 반드시 `git pull` 해주세요!**
- 수정 사항은 **Git 로그 및 코드 주석** 참고
- **Firebase 패키지 설치 필요** → 설치법은 하단 참고
- 📦 **Git LFS 사용 중**: 클론 전 `git lfs install` 필요
- 🛠️ **릴리스 탭에서 빌드된 실행 파일(.exe) 다운로드 가능**  
  → GitHub에 푸시하면 자동으로 릴리스 및 빌드 파일 생성 
---

## ✅ 구현 완료 기능

- 캐릭터별 능력 구현  
- 게임매니저 싱글톤 구현  
- Retry 로직 작동 성공  
- HUD 수정 및 해상도 고정  
- 코드 폴더 구조 정리  
- 체력 스킬 레벨업 시 최대 체력 증가  
- 오디오 매니저 추가  
- 미니맵 구현  

---

## 🧪 추후 구현 예정

- 게임 수치 조정  
- 시연 영상 제작  
- 여러 컴퓨터로 테스트  

---

## 🚧 미구현 기능

- [ ] 플레이어 이름 표시 및 동기화  
- [ ] 풀 매니저 네트워크용 구현  
- [ ] 무기 위치 지연 현상 해결  

---

## ▶️ 게임 작동 방법

1. **로그인**
   - 아이디와 비밀번호 입력  
   - 계정이 없을 경우 자동 회원가입  
   - 기존 계정일 경우, 아이디에 맞는 비밀번호 입력 필수

2. **방 생성 및 참가**
   - 방장이 먼저 `Start Host` 클릭해 방 생성  
   - 이후 참가자는 방장의 IP 주소 입력 후 `Start Client` 클릭해 접속

4. **게임 시작**
   - 캐릭터 선택 시 게임 시작  
   - 이동 조작: 방향키 또는 WASD 키  
   - 몬스터 처치 시 킬 수 및 경험치 증가

5. **레벨업 및 스킬 사용**
   - 레벨업 시 스탯 포인트 증가  
   - 우하단 스탯 버튼 클릭 → 스킬창 열기  
   - 스탯 포인트로 스킬 배분 가능

---

## 🔧 게임 수치 위치 정리

- **몬스터 관련 수치**: `EnemySpawner` 오브젝트
- **무기 관련 수치**:  
  `Data/Scriptable Object` 폴더 또는  
  `Player` 프리팹 내부 `Weapon`, `Gear` 오브젝트

---

## 🔥 Firebase 설치법

1. 아래 링크에서 Firebase SDK for Unity 다운로드  
   🔗 https://firebase.google.com/download/unity

2. 압축 해제 후 Unity에서  
   `Assets > Import Package > Custom Package` 실행

3. 아래 2개 패키지 **임포트**  
   - `FirebaseAuth.unitypackage`  
   - `FirebaseDatabase.unitypackage`

---

## 🧠 ML 구현 변경 사항

- 기존 `StyleEvaluator` 스크립트 삭제  
  → 위치: `Player > Canvas > Style`

- 새로운 스크립트 `PlayerStyleText` 추가  
  → 같은 위치(`Player > Canvas > Style`)에 배치됨

- 기존: 파이썬에서 학습한 모델에 실시간 연결  
- 변경: 결정 트리 모델 기준을 C#에서 직접 구현 (if-else 문 기반)

- 학습 모델은 가져오지 않았으며, **결정 트리의 기준만 반영**
- 서버는 거의 건드리지 않고, **Style UI 오브젝트 내부에서 처리**
- 결과는 Text로만 네트워크 동기화  
- 하단 `OnPlayStyleChanged()` 함수는 ChatGPT 참고하여 작성함 → **확인 부탁드립니다!**
