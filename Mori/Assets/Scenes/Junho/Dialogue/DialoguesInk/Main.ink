// 전역 변수 (평판 등)
VAR day = 1
VAR rep = 0

// 공통 유틸: 태그로 UI에 신호
// #speaker:이름  #portrait:키워드  #scene:씬이름  #sfx:효과음키
// C#에서 tags를 읽어 UI/씬/사운드를 처리

// 방문자 목록 (C#에서 이 knot 이름들 중 3개를 랜덤으로 고른다)
LIST VISITORS = 손님, bob, chloe

=== visitor_alice ===
#speaker:여자2 #portrait:Lady1
...
"안녕하세요"
"연애와 관련된 일도 점 쳐줄 수 있죠?"
#speaker:모리 #partrait:none
* [그렇다]
    "저, 운명의 사람을 만난 것 같아요!"
    ->spk
* [당연하죠]
    "저, 운명의 사람을 만난 것 같아요!"
    ->spk
    
= spk
#speaker:여자2 #portrait:Lady1
"저번 파티 때 만난 신사분인데요, 어찌나 색다르던지!"
"마치 제게 더 넓은 세계를 보여줄 것만 같아요, 그이는."
#speaker:유령 #portrait:Boy1
아는 패턴이군..
#speaker:여자2 #portrait:Lady1
만난 지는 얼마 되지 않았지만... 저는 확신해요!
#speaker:유령 #portrait:Boy1
천생연분이라고?
#speaker:여자2 #portrait:Lady1
우리 둘은 천생연분이라는 걸!
(그녀는 얼굴을 붉히며 급히 말을 마무리 지었다.)
그..그래서 연애 관련해서, 뭐든 봐 주실 수 있을까 해서요.
#speaker:모리 #partrait:none
* [의뢰를 받지 않는다.]
-> no
* [의뢰를 받는다.]
-> END

= no
#speaker:유령 #portrait:Boy1
철 없는 아가씨로구만.
#speaker:여자2 #portrait:Lady1
어, 어떻게 그럴 수가!
#portrait:none
-> END

=== visitor_bob ===
#speaker:밥 #portrait:none
오늘은 뭘 추천해주나요?
* 따뜻한 차를 권한다
    #speaker:밥 #portrait:Boy1
    #scene:Game
    좋아요. 마음이 편안해지네요.
    ~ rep = rep + 1
    -> END
* 차가운 음료를 권한다
    #speaker:밥 #portrait:Boy1
    #scene:Start Scene
    음… 오늘은 추운데요.
    ~ rep = rep - 1
    -> END


=== visitor_chloe ===
#speaker:클로이 #portrait:none
여긴 조용하네요. 잠깐 얘기해도 될까요?
+ [예, 들어드릴게요]
    #speaker:클로이 #portrait:Boy1
    고마워요. 사실…
    #portrait:none
    -> END
+ [죄송해요, 바빠요]
    #speaker:클로이 #portrait:Boy1
    이해해요. 다음에요.
    #portrait:none
    -> END