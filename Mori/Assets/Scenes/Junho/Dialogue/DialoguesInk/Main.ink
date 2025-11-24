VAR event_success = false
VAR event_has_result = false
VAR event_score = 0
VAR event_tag = ""

=== day1_visitor1 ===
#speaker:유령 #portrait:angry_default
..어나.
#speaker:유령
야!
#speaker:유령
일어나!
+ [...]
    -> day1_start

= day1_start
#speaker:유령 #portrait:base_default
정신이 들었나?
* [이곳은 어디?]
    -> day1_start2
* [뭘 하고 있었지]
    -> day1_start2

=day1_start2
#speaker:유령 #portrait:amazed_default
또 이러기야? 요새 자꾸 오락가락 하네.
#speaker:유령 #portrait:base_default
넌 점술사고,
#speaker:유령
나는 너에게 씌인 유령이고.
#speaker:유령
일단... 손님이 오잖아!
#speaker:손님
안녕하세요. 오늘의 운세 봐 주세요!
#speaker:유령 #portrait:smiling_default
다행히 별 거 아니네. 이런 건 네가 가진 '타로 카드'로 해결 할 수 있어.
#speaker:유령 #portrait:smiling_default_1
손님과 대화가 끝나면 타로 카드를 집어보자.
#speaker:손님
괜찮으세요? 아까부터 어딜 보고 계시는...
+ [괜찮다]
    -> okay

= okay
#speaker:유령 #portrait:angry_default
바보야! 눈 저리 돌려. 나는 너한테만 보인단 말야.
#speaker:유령
일단 카드를 집어!
-> END


=== day1_visitor2 ===
#portrait:base_default
의뢰인이 뽑은 카드는 '태양' 카드였다.

#speaker:유령 #portrait:smiling_default
다행히 점술을 하는 법은 대충 기억하고 있는 모양이네. 이게 무슨 뜻인지 알겠어?
+ [아니]
    -> NO

= NO
#speaker:유령
그렇겠지. 점은 네가 치고 설명은 항상 내가 해 줬으니까
#speaker:유령
네가 이 자리에 앉아있는 것도 다 내 덕분이다, 이거야.
+ [...]
    -> card
    
= card
#speaker:유령 #portrait:smiling_default_1
이건 '태양' 카드야. 풍요와 생명력, 밝은 에너지를 상징하지
#speaker:유령 #portrait:base_default_1
아이가 태양 아래에서 말을 타고 놀고 있는게 보여?
#speaker:유령
풍요로운 곳에서 놀이를 즐긴다는 거지.
#speaker:유령
밝고 명랑하다는 느낌을 주는걸.
#speaker:유령
이제 손님에게 결과를 알려줄 차례야
* [위험한 하루]
    -> wrong_answer
* [우울한 하루]
    -> wrong_answer
* [밝고 행복한 하루]
    -> correct_answer

= wrong_answer
#portrait:base_default
그리고-
* [부족함 없이 즐긴다]
    -> wrong_answer1
* [운동을 해야 한다]
    -> wrong_answer1
* [고민의 기로에 선다]
    -> wrong_answer1

= correct_answer
#portrait:base_default
그리고-
* [부족함 없이 즐긴다]
    -> correct_answer1
* [운동을 해야 한다]
    -> wrong_answer1
* [고민의 기로에 선다]
    -> wrong_answer1

= wrong_answer1
#speaker:손님 #portrait:base_default
오늘은 친구들과 피크닉을 가기로 했는데, 정말로 그런 일이 생길까요?
#speaker:손님
걱정이네요. 조심해야겠어요
첫 번째 손님은 걱정스러운 듯한 얼굴로 가게를 나갔다.
유령은 고개를 절레절레 저였다.
-> progress


= correct_answer1
#speaker:손님
정말인가요?
#speaker:손님
오늘은 친구들과 피크닉을 가기로 했거든요, 말처럼 좋은 일만 일어나면 좋겠어요!
첫 번째 손님은 활짝 웃으며 가게를 나갔다.
-> progress



= progress
#speaker:유령 #portrait:base_default
이건 점술이니까, 무슨 일이 일어나든 네 책임도 아니지. 똑같은 소리를 해 줘도 다 다르게 알아듣기 마련이니까.
#speaker:유령
네가 무슨 대답을 하든 돈은 들어올거야.
#speaker:유령
이제 어떻게 하는 지 감이 잡히지?
* [아직 혼란스럽다]
    -> progress1
* [하지만...]
    -> progress1
    
= progress1
#speaker:유령
네가 뭐 대단한 점술가도 아니고, 아직 '타로 카드'를 쓰는 걸로 충분한 손님들이나 올 거야.
#speaker:유령 #portrait:smiling_default
나는 이렇게 옆에서 너를 도와줄 순 있지만, 직접 점을 치는 건 너니까. 힘내봐!
#speaker:유령 #portrait:base_default
앗, 다음 손님이 온 모양이다. 이번엔 내가 설명해주지 않아도 되겠지?

//두번째 손님 시작
#speaker:유령 #portrait:angry_default
그보다 말이지 너, 냅다 점 부터 보지 말란 말야.
#speaker:유령
손님이 어떤 행색을 갖췄는지도 좀 보고, 대화를 해서 정보도 좀 얻고...
#speaker:유령 #portrait:smiling_default
이런 게 다 점술의 결과를 설명 할 때 유용한 정보니까!
+ [사기 수법 같다]
    -> fraud

= fraud
#speaker:유령 #portrait:angry_default
나 원 참... 점술이라는 건 미래를 알려주는 답지가 아니라고.
#speaker:유령
해석이 중요해! 내가 알려주는 걸 잘 해석해 내야지.
#speaker:유령
손님과 대화를 통해 힌트를 얻는 것도 중요한 과정이야.
뭔가 멋들어진 차림을 한 남자가 들어왔다.
#speaker:손님
본론부터 말 할까요? 사실 저도 이런 곳은 처음 와 봐서 말이죠.
#speaker:손님
사업을 하려고 하는데, 나도 처음 뛰어드는 분야라서 불안한 마음이 가시질 않습니다.
#speaker:손님
마음의 평안이라도 얻을 겸, 속는 셈 치고 점이나 보러 온 건데...
제법 우아한 자세를 하고 있지만 반짝거리는 열정이 눈에서 느껴지는 것 같다.
#speaker:유령 #portrait:base_default
제법 멀리에서 온 것 같아. 타고 온 마차라던지...
+ [이 작은 마을에서 볼 법한 사람이 아니다.]
    -> Unique
    
= Unique
#speaker:유령
하긴, 사업을 하는 사람이 점술을 믿고 다닌다고 하면 평판이 어쩌겠어?
#speaker:유령
무슨 사업인지 궁금해지는 걸.
#speaker:손님
하하, 주위에서 걱정을 할 정도면 제가 고민이 심하긴 했던 모양입니다.
#speaker:손님
어쩌면 용기가 필요한 걸지도 모르겠습니다.
-> END

=== day1_visitor3 ===
자, 결과를 보고 해석해 볼까?
{ event_success:
    -> day1_v2_success
- else:
    -> day1_v2_fail
}

= day1_v2_success
#portrait:base_default
남자가 뽑은 카드는 바보의 카드였다.
#speaker:손님
좋은 단어는 아닌 것 같은데요...
#speaker:유령 #portrait:base_default
글쎄, 이 사람에게는 꽤나 좋은 카드 같은데?
#speaker:유령 #portrait:base_default_1
0번. 수리학적으로 텅 비어있는 새로운 시작을 나타내는 카드지.
#speaker:유령
낭떠러지인데도 두려워하지 않는 모험심!
#speaker:유령 #portrait:base_default
나쁘게 말하자면 생각이 없는 거지만...
#speaker:유령
이 사람이 도전하려는 사업 분야가 위험을 동반하는 걸까?
* [사업에 관해 물어본다.]
    -> business
* [사업이 위험을 동반하고 있는가]
    -> business

= business
#speaker:손님 #portrait:base_default
...! 맞습니다. 무역 사업에 도전해 보려고 해요.
#speaker:유령
앞길에 위험이 많이 있네. 정말 위험해! 무역 사업이라면 한두 가지가 아니겠는걸?
#speaker:유령
불리한 상황과 새로운 세계가 같이 있는 카드야. 부정적인 상황이지만 조력자가 함께하고 있음을 보여주기도 하지.
#speaker:유령
이런 사람은 고집이 강해서 앞길이 험해도 굽히지 않을 테니까, 목숨을 걸어서도 포기하지 않을 의지가 중요하겠어.
* [하지만...앞길에 수많은 위험이 보인다. 재고하라고 설득한다.]
    -> N1
* [위험하지만 당신에게 조력자가 있기에 헤쳐나갈 수 있을 것이다.]
    -> N2
* [아직은 때가 아니기 때문에 기다려야 한다. 계획을 세우며 안전히 가야 한다.]
    -> N3

= N1
#portrait:base_default
무역 사업이 위험한 건 알고 있지만, 죽을 만큼의 위험을 감수할 것은 아니지 않은가?
당신은 이 카드가 가진 위험을 설명하며 목숨을 걸어야 할 것 같다며 말렸다.
남자는 조용히 말을 들으며, 고민을 하는 듯 하더니 조용히 인사를 하곤 가게를 나갔다.
#speaker:유령 #portrait:base_default
꽤 흔들린 것 같은 모습이었어! 주위에서 걱정이 많았다더니, 정말인가보네.
#speaker:유령
하지만 저 사람... 슬퍼보인다.
-> visitor3

= N2
#speaker:손님 #portrait:base_default
조력자라...
남자는 흥미로운 듯이 이야기를 들었다.
#speaker:손님
하하, 제 기분 좋으라고 좋은 말만 해 주시는 거 아닌가요? 이거.
+ [타로 카드의 해석에 대해 이야기한다]
    -> analysis

= analysis
#speaker:손님 #portrait:base_default
제법 말이 되는군요. 제 일에 확신이 들기도 합니다.
#speaker:손님
당신에게는 미안하지만... 우리같은 사업가 들은 이런 걸 믿어서는 안 됩니다.
#speaker:손님
하지만 왜 사람들이 점술가를 찾아다니는지 알겠네요! 하하. 고마워요.
남자는 후련한 표정으로 가게를 나갔다.
#speaker:유령 #portrait:base_default
좋은 말만 해 준게 맞긴 해. 어디 가서 다쳐오지나 않으면 다행이겠군. 뭐, 잘 했어!
-> visitor3

= N3
#portrait:base_default
당신은
하지만 뭔가 그럴듯한 해석으로 둘러댄 듯한 느낌. 남자는 뭔가 아리송한 표정을 지으며 고개를 끄덕이고는, 이내 가게를 나갔다.
#speaker:유령 #portrait:base_default
네 말을 들을 것 같지는 않은 걸.
-> visitor3

= day1_v2_fail
남자가 뽑은 카드는 00의 카드였다.
#speaker:유령 #portrait:amazed_default
카드가 보이지 않는데? 오늘 상태가 안 좋은가 보네.
#speaker:유령 #portrait:amazed_default_1
해 줄 수 있는 말이 없는 걸... 이럴 때는 네가 알아서 수습 해야지, 뭐.
* [앞길에 수많은 위험이 보인다. 다른 길을 찾아보자]
    -> apprenticeship
* [아직은 때가 아니기 때문에, 계획을 세우며 안전히 가야 한다.]
    -> apprenticeship
* [할 수 있을 것이라는 응원]
    -> cheering

= apprenticeship
#speaker:손님 #portrait:base_default
뭔가 해석이나 자세한 설명 같은 건 없는 건가요?
* [정확히 보이는 게 없다]
    -> excuse
* [그럴듯한 해석으로 둘러댄다]
    -> excuse

= excuse
#portrait:base_default
남자는 뭔가 아리송한 표정을 지으며 고개를 끄덕이고는, 이내 가게를 나갔다.
#speaker:유령 #portrait:base_default
네 말을 들을 것 같지는 않은 걸.
-> visitor3

= cheering
#speaker:손님 #portrait:base_default
오오, 그런가요? 왜죠?
+ [그럴듯한 해석으로 둘러댄다.]
    -> excuse1

= excuse1
남자는 말을 믿는 눈치는 아니었지만, 마음은 한결 가벼워졌다며 웃으며 가게를 나갔다.
#speaker:유령 #portrait:base_default
우린 이 사람이 무슨 사업을 시작하려고 하는 지도 모르는데 말이지.
-> visitor3


= visitor3
#speaker:유령 #portrait:smiling_default
벌써 피곤해보이는데? 안그래도 빌빌거리는 몸인데, 일찍 들어가서 쉬는 게 좋을 거 같아.
#speaker:유령 #portrait:smiling_default_1
오늘은 다음 손님까지만 받자.
#speaker:손님 #portrait:base_default
...
#speaker:손님
안녕하세요.
#speaker:손님
연애와 관련된 일도 점 쳐줄 수 있죠?
* [그렇다]
    -> YES
* [당연하죠]
    -> YES

= YES
#speaker:손님
저, 운명의 사람을 만난 것 같아요!
#speaker:손님
저번 파티 때 만난 신사문인데요, 어찌나 색다르던지!
#speaker:손님
마치 제게 더 넓은 세계를 보여줄 것만 같아요, 그이는.
#speaker:유령 #portrait:base_default
아는 패턴이군.
#speaker:손님 #portrait:base_default
만난 지는 얼마 되지 않았지만... 저는 확신해요!
#speaker:유령 #portrait:base_default
천생연분이라고?
#speaker:손님 #portrait:base_default
우리 둘은 천생연분이라는 걸!
(그녀는 얼굴을 붉히며 급히 말을 마무리 지었다.)
#speaker:손님
그..그래서 연애 관련해서, 뭐든 봐 주실 수 있을까 해서요.
* [의뢰를 받는다]
    -> END
* [의뢰를 받지 않는다]
    -> do_not_receive_orders

= do_not_receive_orders
#speaker:유령 #portrait:base_default
철 없는 아가씨로구만.
#speaker:손님 #portrait:base_default
어, 어떻게 그럴 수가!
그녀는 부끄러운 듯 얼굴을 가리고는 황급히 자리를 떠났다.
#skip_intermission
-> END


=== day1_visitor4 ===
{ event_success:
    -> day1_v3_success
- else:
    -> day1_v3_fail
}

= day1_v3_fail
#portrait:base_default
딱히 의미있는 결과가 나오지 않았다.
-> branch

= day1_v3_success
#speaker:유령 #portrait:amazed_default
이거이거, '바보' 카드잖아?
#speaker:유령 #portrait:amazed_default_1
항상 있는 스토리잖아, 이거! 꽉 막힌 곳에서 살던 순수한 여자아이가 사기꾼같은 남자에게 푹 빠져버리는...
#speaker:유령 #portrait:smiling_default
그래. 한 번 떠볼까? 그 남자...
* [자유분방한 남자]
    -> fraud
* [규칙에 얽매이지 않는 남자]
    -> fraud
* [제법 고집이 센 남자]
    -> fraud

= fraud
#speaker:손님 #portrait:base_default
어라, 맞아요! 정확하시네요. 정말 예측 할 수 없는 남자에요.
#speaker:손님 #portrait:base_default
정해진 길을 걷지 않는다는 느낌이 좋아요! 고집이 세다기보다는 강단이 있는 편이라고 해주세요.
#speaker:유령 #portrait:base_default
...이건 확실히 말해줄 수 있겠군. 그녀석, 결혼하자고 하면 분명 도망갈거야.
#speaker:유령 #portrait:base_default
겉으로는 화려하지만 껍데기 일 뿐. 책임감 같은 건 전혀 없지.
-> branch1

= branch
#speaker:손님 #portrait:base_default
...어떻게 나왔나요?
* [내가 할 말은 없는 것 같다]
    -> branch2
* [결혼에 찬성]
    -> branch3

= branch1
#speaker:손님 #portrait:base_default
...어떻게 나왔나요?
* [내가 할 말은 없는 것 같다]
    -> branch2
* [결혼에 찬성]
    -> branch3
* [결혼을 재고해 보라]
    -> branch4

= branch2
#speaker:손님 #portrait:base_default
...그 말이 다인가요? 저는 보다, 뭔가, 음...
#speaker:손님 #portrait:base_default
'확실한' 대답이 듣고 싶어서 온 거라구요.
#speaker:손님 #portrait:base_default
...
#speaker:손님 #portrait:base_default
왜 다들 그런 반응인지 모르게썽요. 내 마음은 진심이었는데...!
#speaker:손님 #portrait:base_default
내 주위 사람들도 다 그런 표정을 지었어요...
#speaker:손님 #portrait:base_default
자꾸 이러면 내가 맞는지 모르겠어...
마치 울 것 같은 얼굴로 그녀는 자리를 떴다.
#speaker:유령 #portrait:smiling_default
주위에서도 엄청 말린 모양이네! 하하.
-> END

= branch3
#portrait:base_default
그녀는 마치 자신의 편을 얻은 것 처럼 의기양양한 얼굴로 답했다.
#speaker:손님 #portrait:base_default
역시 그렇죠? 있죠, 연애 운이라면 어떤 것까지 볼 수 있는 거에요?
그 후로 한참동안 이런저런 잡다한 운세놀음에 시달렸다.
어쩌면 대답을 잘못 한 걸지도 모르겠다.
유령이 보기 드물게 짜증을 낼 때쯤에야 그녀는 총총거리며 자리를 떴다.
-> END

= branch4
그녀는 기대하지 않은 결과가 나온 듯 책상을 손으로 치며 외쳤다.
#speaker:손님 #portrait:base_default
어째서죠?! 저, 저는 그런 말을 들으려고 온 게 아니에요!
그 후로 몇 마디 대화를 나누었지만 이 어린 아가씨가 설득 될 기미는 보이지 않았다.
오히려 반항심을 자극한 걸지도 모른다...
#speaker:유령 #portrait:base_default
흥, 시시하네.
-> END

=== day1_end ===
#speaker:유령 #portrait:smiling_default
수고했어! 지친 기색이 보이는걸.
* [피곤해]
    -> GO_END
* [자고싶어]
    -> GO_END

= GO_END
그래, 잘 자.
-> END









