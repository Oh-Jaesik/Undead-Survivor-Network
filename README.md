# 🎮 통합형 실시간 로그라이크 멀티플레이 게임 개발

---

## 👪 개발 멤버 소개

<table>
  <tr>
    <td height="140px" align="center">
      <a href="https://github.com/Oh-Jaesik">
        <img src="https://github.com/Oh-Jaesik.png?size=140" width="140px" /><br><br>
        👑 Oh-Jaesik<br>(Server)
      </a>
    </td>
    <td height="140px" align="center">
      <a href="https://github.com/2hyeoksang">
        <img src="https://github.com/2hyeoksang.png?size=140" width="140px" /><br><br>
        😆 2hyeoksang<br>(AI)
      </a>
    </td>
    <td height="140px" align="center">
      <a href="https://github.com/yjhoo98">
        <img src="https://github.com/yjhoo98.png?size=140" width="140px" /><br><br>
        😶 yjhoo98<br>(DB)
      </a>
    </td>
    <td height="140px" align="center">
      <a href="https://github.com/geeeeeonho">
        <img src="https://github.com/geeeeeonho.png?size=140" width="140px" /><br><br>
        🙄 geeeeeonho<br>(Client)
      </a>
    </td>
    <td height="140px" align="center">
      <a href="https://github.com/haeun91">
        <img src="https://github.com/haeun91.png?size=140" width="140px" /><br><br>
        😊 haeun91<br>(DevOps)
      </a>
    </td>
  </tr>
  <tr>
    <td align="center">네트워크<br/>로직 동기화<br/>UML 설계</td>
    <td align="center">게임 로직<br/>플레이어 성향 분석<br/>ML 모델</td>
    <td align="center">로그인 기능<br/>게임 데이터 수집<br/>DB 업데이트</td>
    <td align="center">맵 구조<br/>UI/HUD 구현</td>
    <td align="center">CI/CD 구현<br/>자동화 빌드<br/>배포 환경<br/>UML 설계</td>
  </tr>
</table>

---

## 📅 프로젝트 기간

2025.03.24. ~ 2025.06.13.

---

## 🛠️ 기술 스택

<img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/Unity-20232A?style=for-the-badge&logo=unity&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/Mirror-000000?style=for-the-badge&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/Firebase-FFCA28?style=for-the-badge&logo=firebase&logoColor=black" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/Git%20LFS-F05032?style=for-the-badge&logo=git&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>
<img src="https://img.shields.io/badge/GitHub%20Actions-2671E5?style=for-the-badge&logo=github-actions&logoColor=white" style="height : auto; margin-left : 10px; margin-right : 10px;"/>

---

## 📝 프로젝트 개요
<img src="https://github.com/user-attachments/assets/c17d554a-25af-43a9-b586-749ed558388c" width="500"/>


- 본 프로젝트는 2025 KEB 미니 프로젝트의 일환으로 진행되었습니다.
- Unity 기반 실시간 멀티플레이어 생존 액션 게임입니다.
- 플레이어들이 협력하여 적을 처치하고 생존하는 것을 목표로 합니다.
- DB와 AI 기능이 구현되어 풍부한 경험을 제공합니다.


---

## ⬇️ 실행 파일 다운로드

- 윈도우 기반의 게임 실행 파일(.exe)을 Releases 탭에서 직접 다운로드하실 수 있습니다.
- 프로젝트의 최신 버전을 간편하게 체험해 보세요!

---

## ▶️ 게임 플레이

1.  **로그인**
    -   아이디와 비밀번호 입력
    -   계정이 없을 경우 자동 회원가입 (아이디 : 메일 형식, 비밀번호 : 6자리 이상)
    -   기존 계정은 아이디에 맞는 비밀번호 입력 필수

2.  **방 생성 및 참가**
    -   방장이 먼저 `Start Host` 클릭해 방 생성
    -   이후 참가자는 방장의 IP 주소 입력 후 `Start Client` 클릭해 접속

4.  **게임 시작**
    -   캐릭터 선택 시 게임 시작
    -   이동 조작: 방향키 또는 WASD 키
    -   몬스터 처치 시 킬 수 및 경험치 증가

5.  **레벨업 및 스킬 사용**
    -   레벨업 시 스탯 포인트 증가
    -   우하단 스탯 버튼 클릭 → 스킬창 열기
    -   스탯 포인트로 스킬 배분 가능

---

## ⚠️ 작업 전 필독

-   **Unity 버전**: `6.1.7f1`
-   **작업 전 반드시 `git pull` 해주세요!**
-   수정 사항은 **Git 로그 및 코드 주석** 참고
-   **Firebase 패키지 설치 필요** → 설치법은 하단 참고
-   📦 **Git LFS 사용 중**: 클론 전 `git lfs install` 필요
-   GitHub **main** branch에 푸시하면 자동으로 빌드 파일 생성 및 릴리스

---

## 🔧 게임 수치 조정

-   **몬스터 관련 수치** : `EnemySpawner` 오브젝트
-   **무기 관련 수치** :
    `Data/Scriptable Object` 폴더 &
    `Player` 프리팹 내부 `Weapon`, `Gear` 오브젝트

---

## 🔥 Firebase 설치법

1.  아래 링크에서 Firebase SDK for Unity 다운로드
    🔗 https://firebase.google.com/download/unity

2.  압축 해제 후 Unity에서
    `Assets > Import Package > Custom Package` 실행

3.  아래 2개 패키지 **임포트**
    -   `FirebaseAuth.unitypackage`
    -   `FirebaseDatabase.unitypackage`

---
