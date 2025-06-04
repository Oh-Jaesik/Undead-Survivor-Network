ML 구현 코드 바뀐 점
- 기존의 StyleEvaluator 스크립트는 Player - Canvas - Style 에서 삭제
- 기존 스크립트는 Python에다 연결해서 바로 그 모델에 학습시켜서 적용시킨 것.
- 새로운 PlayerStyleText 라는 스크립트 생성해서 Player = Canvas - Style에 붙임.
- 학습 모델이 결정 나무 모델이라서 if-else 문으로 확인할 수 있다고 C#에서도 구현이 가능하다고 합니다..
- 모델을 가져와서 학습을 시킨건 아니고, 파이썬 코드에서 학습시킨 모델에서 각 노드에서 구분하는 기준을 그대로 가져와서 적용..? 시킨거입니다
- 최대한 서버 안건드릴 수 있게 Style UI 오브젝트에다가 다 집어넣어서 text 결과만 동기화해놓..?긴 했는데 PlayerStyleText 하나만 추가한거라 확인 한번만 부탁드릴게영
- 맨 아래에 OnPlayStyleChanged는 챗지피티한테 물어보고하다가 어쩌다 작성됐는데.. 괜찮겠죠?
