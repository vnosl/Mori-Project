// 전역 변수 (평판 등)
VAR day = 1
VAR rep = 0

// 공통 유틸: 태그로 UI에 신호
// #speaker:이름  #portrait:키워드  #scene:씬이름  #sfx:효과음키
// C#에서 tags를 읽어 UI/씬/사운드를 처리

// 방문자 목록 (C#에서 이 knot 이름들 중 3개를 랜덤으로 고른다)
LIST VISITORS = 손님, bob, chloe

=== visitor_alice ===
#speaker:손님 #portrait:alice_happy
안녕하세요! 오늘 하루는 어땠나요?
* (좋았다) 
    #sfx:select
    좋았어요! 기분이 상쾌하네요.
    -> alice_branch_good
* (별로다)
    그럭저럭이요…
    -> alice_branch_bad

= alice_branch_good
#speaker:손님 #portrait:alice_smile
정말 다행이네요. 혹시 이 물건에 대해 알고 싶나요?
+ [물건 설명을 듣는다]
    #speaker:손님
    이건 특별한 아이템이에요.
    ~ rep = rep + 1
    -> END
+ [지금은 괜찮다]
    #speaker:손님
    알겠어요. 다음에요!
    -> END

= alice_branch_bad
#speaker:손님 #portrait:alice_sad
그런 날도 있죠… 힘내세요.
-> END


=== visitor_bob ===
#speaker:밥 #portrait:bob_normal
오늘은 뭘 추천해주나요?
* 따뜻한 차를 권한다
    #speaker:밥 
    #scene:Game
    좋아요. 마음이 편안해지네요.
    ~ rep = rep + 1
    -> END
* 차가운 음료를 권한다
    #speaker:밥 
    #scene:Start Scene
    음… 오늘은 추운데요.
    ~ rep = rep - 1
    -> END


=== visitor_chloe ===
#speaker:클로이 #portrait:chloe_think #scene:BackRoom
여긴 조용하네요. 잠깐 얘기해도 될까요?
+ [예, 들어드릴게요]
    #speaker:클로이
    고마워요. 사실…
    -> END
+ [죄송해요, 바빠요]
    #speaker:클로이
    이해해요. 다음에요.
    -> END