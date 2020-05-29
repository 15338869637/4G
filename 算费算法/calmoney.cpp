namespace camera {
namespace charge {
typedef unsigned char uchar;
typedef unsigned short uint;
typedef unsigned int ulong;

#define HexBcd(Hex) (((Hex / 10) << 4) + (Hex % 10))
#define BcdHex(Bcd) ((((Bcd >> 4) & 0x0f) * 0x0a) + (Bcd & 0x0f))

uchar consume[3], tmpmoney[3], block2[16], time[7], tptime[7], buf[256];
uint stopday;
uchar stophour, stopmin, mianfei;

void Memcpy(uchar s[], uchar e[], uchar cnt) {
  uchar i;
  for (i = 0; i < cnt; i++) {
    s[i] = e[i];
  }
}

char Concmp(uchar arry[], uchar constant, uchar cnt) {
  uchar i, sta = 0;
  for (i = 0; i < cnt; i++) {
    if (arry[i] != constant) {
      sta = 1;
      break;
    }
  }
  return sta;
}

char Arrycmp(uchar a1[], uchar a2[], uchar cnt) {
  uchar i, sta = 0;
  for (i = 0; i < cnt; i++) {
    if ((a1[i] < a2[i]) || (a1[i] == a2[i])) {
      if (a1[i] < a2[i]) {
        sta = 1;
        return sta;
      }
    } else {
      return sta;
    }
  }
  return sta;
}

void Conzero(uchar arry[], uchar constant, uchar cnt) {
  uchar i;
  for (i = 0; i < cnt; i++) arry[i] = constant;
}

uint CalDay(uchar month) {
  uint arry[13] = {0x00, 0x00, 0x1f, 0x3B,  0x5A,  0x78, 0x97,
                   0xb5, 0xd4, 0xf3, 0x111, 0x130, 0x14e};
  uint Day;
  Day = arry[month];
  return Day;
}

uchar ParkTime(void) {
  uint in_day, out_day;
  uchar in_hour, in_min, in_sec, out_hour, out_min, out_sec, ok = 1;

  out_day = CalDay(BcdHex(time[2])) + BcdHex(time[3]);
  if (((BcdHex(time[1]) & 0x03) == 0) & (BcdHex(time[2]) > 2))
    out_day += 1;  //出场年为瑞年且大于2月
  out_hour = BcdHex(time[4]);
  out_min = BcdHex(time[5]);
  out_sec = BcdHex(time[6]);

  in_day = CalDay(BcdHex(block2[2])) + BcdHex(block2[3]);
  if (((BcdHex(block2[1]) & 0x03) == 0) && (BcdHex(block2[2]) > 2))
    in_day += 1;  //入场年为瑞年且大于2月
  in_hour = BcdHex(block2[4]);
  in_min = BcdHex(block2[5]);
  in_sec = BcdHex(block2[6]);

  if (time[1] > block2[1]) {
    if ((BcdHex(block2[1]) & 0x03) == 0)
      out_day += 366;
    else
      out_day += 365;
  } else if (time[1] < block2[1]) {
    return ok;
  }

  if (out_day < in_day) return ok;
  if (out_day == in_day) {
    if (out_hour < in_hour) return ok;
    if (out_hour == in_hour) {
      if (out_min < in_min) return ok;
      if (out_min == in_min) {
        if ((out_sec < in_sec) || (out_sec == in_sec)) return ok;
      }
    }
  }

  stopday = 0;
  stophour = 0;
  stopmin = 0;
  if (out_sec > in_sec) {
    out_min += 1;
    if (out_min == 60) {
      out_min = 0;
      out_hour += 1;
      if (out_hour == 24) {
        out_hour = 0;
        out_day += 1;
      }
    }
  }

  while (in_min != out_min) {
    in_min += 1;
    if (in_min == 60) {
      in_min = 0;
      in_hour += 1;
      if (in_hour == 24) {
        in_hour = 0;
        in_day += 1;
      }
    }
    stopmin += 1;
  }
  while (in_hour != out_hour) {
    in_hour += 1;
    if (in_hour == 24) {
      in_hour = 0;
      in_day += 1;
    }
    stophour += 1;
  }
  while (in_day != out_day) {
    in_day += 1;
    stopday += 1;
  }
  ok = 0;
  return ok;
}

/******************************************************
 *t[0]为入场或出场时，t[1]为入场或出场分
 *buff[0],buff[1]为第一时间段起始时、分
 *buff[2],buff[3]为第二时间段起始时、分
 *buff[4],buff[5]为第三时间段起始时、分
 *返回所属时间段(低4位)和时间段数(高四位)
 *******************************************************/
uchar Timeslice3(uchar t[]) {
  uchar s, d, flag = 1;
  uchar tmp1[2], tmp2[6];
  tmp1[0] = BcdHex(t[0]);
  tmp1[1] = BcdHex(t[1]);
  tmp2[0] = BcdHex(buf[0]);
  tmp2[1] = BcdHex(buf[1]);
  tmp2[2] = BcdHex(buf[2]);
  tmp2[3] = BcdHex(buf[3]);
  tmp2[4] = BcdHex(buf[4]);
  tmp2[5] = BcdHex(buf[5]);

  while (1) {
    if (tmp1[0] == tmp2[0]) {
      if (flag) {
        if ((tmp1[1] > tmp2[1]) || (tmp1[1] == tmp2[1]))
          s = 1;
        else
          s = 3;
      } else {
        s = 3;
      }
      break;
    } else if (tmp1[0] == tmp2[2]) {
      if (flag) {
        if ((tmp1[1] > tmp2[3]) || (tmp1[1] == tmp2[3]))
          s = 2;
        else
          s = 1;
      } else {
        s = 1;
      }
      break;
    } else if (tmp1[0] == tmp2[4]) {
      if (flag) {
        if (tmp1[1] < tmp2[5])
          s = 2;
        else
          s = 3;
      } else {
        s = 2;
      }
      break;
    } else {
      flag = 0;
      tmp1[0] += 1;
      if (tmp1[0] == 24) tmp1[0] = 0;
    }
  }

  d = 3;
  if (buf[4] == 0xff) {
    if (s == 3) s = 2;
    d = 2;
  }
  s = s | (d << 4);
  return s;
}

/******************************************************
 *t[0]为入场或出场时，t[1]为入场或出场分
 *buff[1],buff[2]为白天段起始时、分
 *buff[3],buff[4]为白天段结束时、分
 *返回所属时间段
 *******************************************************/
uchar Timeslice2(uchar t[]) {
  uchar s, flag = 1;
  uchar tmp1[2], tmp2[4];
  tmp1[0] = BcdHex(t[0]);
  tmp1[1] = BcdHex(t[1]);
  tmp2[0] = BcdHex(buf[1]);
  tmp2[1] = BcdHex(buf[2]);
  tmp2[2] = BcdHex(buf[3]);
  tmp2[3] = BcdHex(buf[4]);

  while (1) {
    if (tmp1[0] == tmp2[0]) {
      if (flag) {
        if ((tmp1[1] > tmp2[1]) | (tmp1[1] == tmp2[1]))
          s = 1;
        else
          s = 2;
      } else {
        s = 2;
      }
      break;
    } else if (tmp1[0] == tmp2[2]) {
      if (flag) {
        if (tmp1[1] < tmp2[3])
          s = 1;
        else
          s = 2;
      } else {
        s = 1;
      }
      break;
    } else {
      flag = 0;
      tmp1[0] += 1;
      if (tmp1[0] == 24) tmp1[0] = 0;
    }
  }
  return s;
}

/******************************************************
 *第9种收费标准计算白天段或晚上段收费金额
 *t[0]\t[1]为白天段或晚上段停留时分
 *bu为参数:白天段或晚上段停车时间bu[0]个小时(含)内每bu[1]个半小时收费bu[2]元
 *         白天段或晚上段停车时间超过bu[0]个小时但在bu[3]个小时(含)内每bu[4]个半小时收费bu[5]元（1,1,1）
 *         白天段或晚上段停车时间超过bu[3]个小时每buff[6]个半小时收费buff[7]元（1,1）
 *         白天段或晚上段最高收费为bu[8]/bu[9]
 *cs为可超时时间
 *canshu为设置参数
 *feel为免费时间
 *******************************************************/
uint Calfeel(uchar t[], uchar bu[], uchar cs, uchar canshu, uchar feel) {
  uchar flag, temp, n;
  uint m, p;

  m = 0;
  if ((t[0] != 0) || (t[1] != 0))  // t[0]\t[1]为白天段或晚上段停留时分
  {
    flag = 1;
    if ((canshu & 0x06) == 0x06) {  //白天段或晚上段都有免费分钟
      if ((stopday == 0)) {
        if (t[0] < (feel / 60))  // feel为免费时间
        {
          flag = 0;
        } else if (t[0] == (feel / 60)) {
          if ((t[1] < (feel % 60)) || (t[1] == (feel % 60))) {
            flag = 0;
          }
        }
      }
    }
    if (flag &&
        ((canshu & 0x0e) == 0x0e)) {  //白天段或晚上段需要去掉免费分钟计算
      if (((t[0] * 60) + t[1]) < feel) {
        t[0] = 0;
        t[1] = 0;
        flag = 0;
      } else {
        if (t[1] < (feel % 60)) {
          t[1] = t[1] + (60 - (feel % 60));
          t[0] = t[0] - (feel / 60) - 1;
        } else {
          t[1] = t[1] - (feel % 60);
          t[0] = t[0] - (feel / 60);
        }
      }
    }

    if (flag) {
      n = t[0] * 2;
      if ((t[1] > 30) || (t[1] == 30)) {
        n += 1;
        t[1] = t[1] - 30;
      }

      if (n < (bu[0] * 2)) {
        temp =
            n;  //白天段或晚上段停车时间bu[0]个小时(含)内每bu[1]个半小时收费bu[2]元
        while (bu[1] && temp) {
          if ((temp > bu[1]) || (temp == bu[1])) {
            temp -= bu[1];
            m += bu[2];
          } else {
            break;
          }
        }
        if ((temp * 30 + t[1]) > BcdHex(cs)) {  //超时
          m += bu[2];
        }
      } else {
        temp =
            bu[0] *
            2;  //白天段或晚上段停车时间bu[0]个小时(含)内每bu[1]个半小时收费bu[2]元
        while (bu[1] && temp) {
          if ((temp > bu[1]) || (temp == bu[1])) {
            temp -= bu[1];
            m += bu[2];
          } else {
            m += bu[2];
            break;
          }
        }
        if (n <
            (bu[3] *
             2)) {  //白天段或晚上段停车时间超过bu[0]个小时但在bu[3]个小时(含)内每bu[4]个半小时收费bu[5]元（1,1,1）
          temp = (n - bu[0] * 2);
          while (bu[4] && temp) {
            if ((temp > bu[4]) || (temp == bu[4])) {
              temp -= bu[4];
              m += bu[5];
            } else {
              break;
            }
          }
          if ((temp * 30 + t[1]) > BcdHex(cs)) {  //超时
            m += bu[5];
          }
        } else {  //白天段或晚上段停车时间超过bu[0]个小时但在bu[3]个小时(含)内每bu[4]个半小时收费bu[5]元（1,1,1）
          temp = ((bu[3] * 2) - (bu[0] * 2));
          while (bu[4] && temp) {
            if ((temp > bu[4]) || (temp == bu[4])) {
              temp -= bu[4];
              m += bu[5];
            } else {
              m += bu[5];
              break;
            }
          }
          //白天段或晚上段停车时间超过bu[3]个小时每bu[6]个半小时收费bu[7]元（1,1）
          temp = (n - bu[3] * 2);
          while (bu[6] && temp) {
            if ((temp > bu[6]) || (temp == bu[6])) {
              temp -= bu[6];
              m += bu[7];
            } else {
              break;
            }
          }
          if ((temp * 30 + t[1]) > BcdHex(cs)) {  //超时
            m += bu[7];
          }
        }
      }

      p = ((bu[8] & 0x00ff) << 8) + (bu[9] & 0xff);
      if (m > p)  //白天段或晚上段最高收费为bu[8]/bu[9]
      {
        m = p;
      }
    }
  }
  return m;
}

/******************************************************
 *第10种收费标准计算白天段或晚上段收费金额
 *t[0]\t[1]为白天段或晚上段停留时分
 *bu为参数:白天段或晚上段免费分钟bu[0],首bu[1]个15分钟收费bu[2]bu[3]元,以后每bu[4]个15分钟收费bu[5]bu[6]元，超时时间bu[7]分钟，该段最高收费bu[8]bu[9]元
 *cs为可超时时间
 *feel为免费时间
 *canshu为设置参数
 *Fir=1,无前N个15分钟收费金额收费金额
 *******************************************************/
uint CalfeelA(uchar t[], uchar bu[], uchar canshu, uchar canshu2, uchar Fir) {
  uchar flag, n, feel, cs, b1[4];
  uint m, p;
  uchar cstime;

  feel = bu[0];
  cs = bu[7];
  m = 0;
  if ((t[0] != 0) || (t[1] != 0))  // t[0]\t[1]为白天段或晚上段停留时分
  {
    flag = 1;
    if (canshu & 0x02) {  //白天段或晚上段都有免费分钟
      if ((stopday == 0)) {
        if (t[0] < (feel / 60))  // feel为免费时间
        {
          flag = 0;
        } else if (t[0] == (feel / 60)) {
          if ((t[1] < (feel % 60)) || (t[1] == (feel % 60))) {
            flag = 0;
          }
        }
      }
    }

    if (flag && (canshu & 0x04) &&
        (canshu & 0x02)) {  //白天段或晚上段需要去掉免费分钟计算
      if (((t[0] * 60) + t[1]) < feel) {
        t[0] = 0;
        t[1] = 0;
        flag = 0;
      } else {
        if (t[1] < (feel % 60)) {
          t[1] = t[1] + (60 - (feel % 60));
          t[0] = t[0] - (feel / 60) - 1;
        } else {
          t[1] = t[1] - (feel % 60);
          t[0] = t[0] - (feel / 60);
        }
      }
    }

    if (flag) {
      n = t[0] * 4 + (t[1] / 15);
      t[1] = t[1] % 15;
      if (Fir) {
        b1[1] = 0;
        b1[2] = 0;
        b1[3] = 0;
      } else {
        b1[1] = bu[1];
        b1[2] = bu[2];
        b1[3] = bu[3];
      }

      if (canshu2 & 0x10) {
        cstime = cs;  //超时时间按16进制
      } else {
        cstime = BcdHex(cs);  //超时时间按10进制
      }  // 2018.07.27修改,如果时间小于或等于超时时间不计费
      if (((n > b1[1]) || (n == b1[1])) &&
          ((n * 15 + t[1]) >
           cstime)) {  //白天段或晚上段首bu[1]个15分钟收费bu[2]bu[3]元
        m += (b1[2] * 256 + b1[3]);
        n -= b1[1];
        while (((n > bu[4]) || (n == bu[4])) && bu[4] &&
               ((n * 15 + t[1]) > cstime)) {
          n = n - bu[4];
          m += (bu[5] * 256 + bu[6]);  //以后每M个15分钟收费金额
        }
        if ((n * 15 + t[1]) > cstime) {  //超时
          m += (bu[5] * 256 + bu[6]);
        }
      } else {
        if ((n * 15 + t[1]) > cstime) m += (bu[2] * 256 + bu[3]);
      }
      p = ((bu[8] & 0x00ff) << 8) + (bu[9] & 0xff);
      if (m > p)  //白天段或晚上段最高收费为bu[8]/bu[9]
      {
        m = p;
      }
    }
  }
  return m;
}

/******************************************************
 *s为入场所属时间段
 *t[0]\t[1]为在入场段到分割断之间停留时分，t[2]\t[3]为在分割断到出场时间之间停留时分
 *ru[0]\ru[1]为入场时分，chu[0]\chu[1]为出场时分
 *******************************************************/
void Threeslicetime(uchar s, uchar t[], uchar ru[], uchar chu[]) {
  uchar t1[4], t2[2], t3[2];

  t2[0] = BcdHex(ru[0]);
  t2[1] = BcdHex(ru[1]);

  t3[0] = BcdHex(chu[0]);
  t3[1] = BcdHex(chu[1]);

  if (s == 1) {  //白天段
    t1[0] = BcdHex(buf[1]);
    t1[1] = BcdHex(buf[2]);
    t1[2] = BcdHex(buf[3]);
    t1[3] = BcdHex(buf[4]);
  } else {  //晚上段
    t1[0] = BcdHex(buf[3]);
    t1[1] = BcdHex(buf[4]);
    t1[2] = BcdHex(buf[1]);
    t1[3] = BcdHex(buf[2]);
  }
  Conzero(t, 0, 4);

  while (t2[1] != t1[3]) {
    t2[1] += 1;
    if (t2[1] == 60) {
      t2[1] = 0;
      t2[0] += 1;
      if (t2[0] == 24) t2[0] = 0;
    }
    t[1] += 1;
  }
  while (t2[0] != t1[2]) {
    t2[0] += 1;
    if (t2[0] == 24) t2[0] = 0;
    t[0] += 1;
  }
  //=========
  if (chu[2] != 0)  //后期增加20140827
    t[3] += 1;

  while (t1[1] != t3[1]) {
    t1[1] += 1;
    if (t1[1] == 60) {
      t1[1] = 0;
      t1[0] += 1;
      if (t1[0] == 24) t1[0] = 0;
    }
    t[3] += 1;
  }
  while (t1[0] != t3[0]) {
    t1[0] += 1;
    if (t1[0] == 24) t1[0] = 0;
    t[2] += 1;
  }
}

/******************************************************
 *s[0]为入场所属时间段，s[1]为晚上所属时间段
 *t[0]\t[1]为在白天段停留时分，t[2]\t[3]为在晚上段停留时分
 *******************************************************/
void Twoslicetime(uchar s[], uchar t[]) {
  uchar ru[2], chu[2], bai[2], wan[2], sli[2];
  uchar t1[4], t2[2];
  ru[0] = BcdHex(block2[4]);
  ru[1] = BcdHex(block2[5]);
  chu[0] = BcdHex(time[4]);
  chu[1] = BcdHex(time[5]);
  t[0] = 0;
  t[1] = 0;
  t[2] = 0;
  t[3] = 0;
  t2[0] = 0;
  t2[1] = 0;
  bai[0] = 0, bai[1] = 0;
  wan[0] = 0;
  wan[1] = 0;

  t1[0] = BcdHex(buf[1]);
  t1[1] = BcdHex(buf[2]);
  t1[2] = BcdHex(buf[3]);
  t1[3] = BcdHex(buf[4]);

  while (t1[1] != t1[3]) {
    t1[1] += 1;
    if (t1[1] == 60) {
      t1[1] = 0;
      t1[0] += 1;
      if (t1[0] == 24) t1[0] = 0;
    }
    bai[1] += 1;
  }
  while (t1[0] != t1[2]) {
    t1[0] += 1;
    if (t1[0] == 24) t1[0] = 0;
    bai[0] += 1;
  }
  t1[0] = BcdHex(buf[1]);
  t1[1] = BcdHex(buf[2]);
  t1[2] = BcdHex(buf[3]);
  t1[3] = BcdHex(buf[4]);
  while (t1[1] != t1[3]) {
    t1[3] += 1;
    if (t1[3] == 60) {
      t1[3] = 0;
      t1[2] += 1;
      if (t1[2] == 24) t1[2] = 0;
    }
    wan[1] += 1;
  }
  while (t1[0] != t1[2]) {
    t1[2] += 1;
    if (t1[2] == 24) t1[2] = 0;
    wan[0] += 1;
  }

  buf[120] = bai[0];  //备用
  buf[121] = bai[1];
  buf[122] = wan[0];
  buf[123] = wan[1];

  if (s[0] == 0x01) {
    sli[0] = BcdHex(buf[3]);
    sli[1] = BcdHex(buf[4]);
  } else {
    sli[0] = BcdHex(buf[1]);
    sli[1] = BcdHex(buf[2]);
  }
  while (ru[1] != sli[1]) {
    ru[1] += 1;
    if (ru[1] == 60) {
      ru[1] = 0;
      ru[0] += 1;
      if (ru[0] == 24) ru[0] = 0;
    }
    t2[1] += 1;
  }
  while (ru[0] != sli[0]) {
    ru[0] += 1;
    if (ru[0] == 24) ru[0] = 0;
    t2[0] += 1;
  }

  if ((t2[0] > stophour) ||
      ((t2[0] == stophour) &&
       ((t2[1] > stopmin) || (t2[1] == stopmin))))  //以前有错
  {
    t2[0] = stophour;
    t2[1] = stopmin;
    if (s[0] == 1) {
      t[0] = t2[0];
      t[1] = t2[1];
    } else {
      t[2] = t2[0];
      t[3] = t2[1];
    }
  } else {
    if (s[0] == s[1]) {
      if (s[0] == 1) {
        t[2] = wan[0];
        t[3] = wan[1];
        t2[0] = t[2];
        t2[1] = t[3];
        while (t2[1] != stopmin) {
          t2[1] += 1;
          if (t2[1] == 60) {
            t2[1] = 0;
            t2[0] += 1;
            if (t2[0] == 24) t2[0] = 0;
          }
          t[1] += 1;
        }
        while (t2[0] != stophour) {
          t2[0] += 1;
          if (t2[0] == 24) t2[0] = 0;
          t[0] += 1;
        }
      } else {
        t[0] = bai[0];
        t[1] = bai[1];
        t2[0] = t[0];
        t2[1] = t[1];
        while (t2[1] != stopmin) {
          t2[1] += 1;
          if (t2[1] == 60) {
            t2[1] = 0;
            t2[0] += 1;
            if (t2[0] == 24) t2[0] = 0;
          }
          t[3] += 1;
        }
        while (t2[0] != stophour) {
          t2[0] += 1;
          if (t2[0] == 24) t2[0] = 0;
          t[2] += 1;
        }
      }
    } else {
      if (s[0] == 1) {
        t[0] = t2[0];
        t[1] = t2[1];

        if ((time[6] < block2[6]) && (time[6] != 0))  //后期增加20140828
          t[3] += 1;

        while (t2[1] != stopmin) {
          t2[1] += 1;
          if (t2[1] == 60) {
            t2[1] = 0;
            t2[0] += 1;
            if (t2[0] == 24) t2[0] = 0;
          }
          t[3] += 1;
        }
        while (t2[0] != stophour) {
          t2[0] += 1;
          if (t2[0] == 24) t2[0] = 0;
          t[2] += 1;
        }
      } else {
        t[2] = t2[0];
        t[3] = t2[1];

        if ((time[6] < block2[6]) && (time[6] != 0))  //后期增加20140828
          t[1] += 1;

        while (t2[1] != stopmin) {
          t2[1] += 1;
          if (t2[1] == 60) {
            t2[1] = 0;
            t2[0] += 1;
            if (t2[0] == 24) t2[0] = 0;
          }
          t[1] += 1;
        }
        while (t2[0] != stophour) {
          t2[0] += 1;
          if (t2[0] == 24) t2[0] = 0;
          t[0] += 1;
        }
      }
    }
  }
}

/******************************************************
 *
 *******************************************************/
void MinAdd(uchar x) {
  uchar t[2];
  t[0] = BcdHex(tptime[3]);
  t[1] = BcdHex(tptime[4]);

  t[1] += x;
  if (t[1] > 59) {
    t[1] -= 60;
    t[0] += 1;
    if (t[0] > 23) {
      t[0] -= 24;
    }
  }
  tptime[3] = HexBcd(t[0]);
  tptime[4] = HexBcd(t[1]);
}

uchar Overtime(uchar t[]) {
  uchar t1[2], t2[2], t3[2], t4[2];
  t1[0] = BcdHex(buf[((buf[101] & 0x0f) - 1) * 2]);  //出场时间段的起始时
  t1[1] = BcdHex(buf[((buf[101] & 0x0f) - 1) * 2 + 1]);  //出场时间段的起始分
  t2[0] = BcdHex(time[4]);                               //出场时
  t2[1] = BcdHex(time[5]);                               //出场分
  t3[0] = BcdHex(t[0]);                                  //限制时
  t3[1] = BcdHex(t[1]);                                  //限制分
  t4[0] = 0;                                             //停车时
  t4[1] = 0;                                             //停车分
  while (t1[1] != t2[1]) {
    t1[1] += 1;
    if (t1[1] == 60) {
      t1[1] = 0;
      t1[0] += 1;
      if (t1[0] == 24) {
        t1[0] = 0;
      }
    }
    t4[1] += 1;
  }
  while (t1[0] != t2[0]) {
    t1[0] += 1;
    if (t1[0] == 24) {
      t1[0] = 0;
    }
    t4[0] += 1;
  }
  if ((t3[0] > t4[0]) || ((t3[0] == t4[0]) && (t3[1] > t4[1])))
    return 0;  //未超时
  else
    return 1;  //超时
}

void DayAdd(void) {
  uchar t1[3];
  uchar month_tab[13] = {00, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
  t1[0] = BcdHex(tptime[0]);
  t1[1] = BcdHex(tptime[1]);
  t1[2] = BcdHex(tptime[2]);

  t1[2] += 1;
  if (month_tab[t1[1]] == t1[2]) {
    if (t1[1] == 2)  //是否为2月
    {
      if (t1[0] & 0x03)  //判断是否为闰年
      {
        t1[2] = 1;
        t1[1] += 1;
        if (t1[1] == 13) {
          t1[1] = 1;
          t1[0] += 1;
        }
      }
    }
  } else if (month_tab[t1[1]] < t1[2]) {
    t1[2] = 1;
    t1[1] += 1;
    if (t1[1] == 13) {
      t1[1] = 1;
      t1[0] += 1;
    }
  }
  if (tptime[6] == 7)  //判断星期
    tptime[6] = 1;
  else
    tptime[6] += 1;

  tptime[0] = HexBcd(t1[0]);
  tptime[1] = HexBcd(t1[1]);
  tptime[2] = HexBcd(t1[2]);
}

void HourAdd(void) {
  uchar t;
  t = BcdHex(tptime[3]);
  t += 1;
  if (t != 24) {
    tptime[3] = HexBcd(t);
    return;
  }
  tptime[3] = 0;
  DayAdd();
}

void MinuteAdd(void) {
  uchar t0;
  t0 = BcdHex(tptime[4]);
  t0 += 1;
  if (t0 != 60) {
    tptime[4] = HexBcd(t0);
    return;
  }
  tptime[4] = 0;
  HourAdd();
}

void DaySub(void) {
  uchar t1[3];
  uchar month_tab[13] = {00, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
  t1[0] = BcdHex(tptime[0]);
  t1[1] = BcdHex(tptime[1]);
  t1[2] = BcdHex(tptime[2]);

  if (t1[2] > 1) {
    t1[2] -= 1;
  } else {
    if (t1[1] > 1) {
      t1[1] -= 1;
      if (t1[1] == 2)  //是否为2月
      {
        if (t1[0] & 0x03)  //判断是否为闰年
        {                  //不是闰年
          t1[2] = 28;
        } else {
          t1[2] = 29;
        }
      } else {
        t1[2] = month_tab[t1[1]];
      }
    } else {
      t1[2] = 31;
      t1[1] = 12;
      t1[0] -= 1;
    }
  }

  if (tptime[6] > 1)  //判断星期
    tptime[6] -= 1;
  else
    tptime[6] = 7;

  tptime[0] = HexBcd(t1[0]);
  tptime[1] = HexBcd(t1[1]);
  tptime[2] = HexBcd(t1[2]);
}

void HourSub(void) {
  uchar t;
  t = BcdHex(tptime[3]);
  if (t != 00) {
    t -= 1;
    tptime[3] = HexBcd(t);
    return;
  }
  tptime[3] = 0x23;
  DaySub();
}

void MinuteSub(void) {
  uchar t0;
  t0 = BcdHex(tptime[4]);
  if (t0 != 00) {
    t0 -= 1;
    tptime[4] = HexBcd(t0);
    return;
  }
  tptime[4] = 0x59;
  HourSub();
}
//其中春节的调休日为4天，实际假日为7天   （不够补0）
// 5月的调休日为2天，实际假日为7天
// 10月的调休日为2天，实际假日为7天
//元旦的调休日为2天，实际假日为3天
//返回1为工作日
uchar workday(void) {
  uchar i;

  for (i = 0; i < 10; i++) {
    if ((tptime[0] == buf[200 + i * 3]) &&
        (tptime[1] == buf[200 + i * 3 + 1]) &&
        (tptime[2] == buf[200 + i * 3 + 2]))
      return 1;
  }

  if ((tptime[6] == 6) || (tptime[6] == 7)) return 0;

  for (i = 10; i < 34; i++) {
    if ((tptime[0] == buf[200 + i * 3]) &&
        (tptime[1] == buf[200 + i * 3 + 1]) &&
        (tptime[2] == buf[200 + i * 3 + 2]))
      return 0;
  }

  return 1;
}

uchar sameday(void) {
  if ((tptime[0] == time[1]) && (tptime[1] == time[2]) &&
      (tptime[2] == time[3]))
    return 1;
  else
    return 0;
}

uint ADDENDTIME(void) {
  uint n = 0;
  uchar t[2];
  if (tptime[3] > time[4]) return n;
  if (tptime[3] == time[4]) {
    if ((tptime[4] > time[5]) || (tptime[4] == time[5])) return n;
  }
  if (workday()) {
    while (1)  //工作日处理
    {
      buf[100] = Timeslice2(tptime + 3);
      if ((buf[100] & 0x0f) == 1)  //返回01H，高峰时段;02H，非高峰时段
      {
        n += buf[11];  //高峰段每半小时收费金额
        t[0] = BcdHex(tptime[3]);
        t[1] = BcdHex(tptime[4]);
        t[1] += 30;
        if (t[1] > 59) {
          t[1] -= 60;
          t[0] += 1;
        }
        tptime[3] = HexBcd(t[0]);
        tptime[4] = HexBcd(t[1]);
      } else {
        n += buf[12];  //非高峰时段：入场分钟+60h
        t[0] = BcdHex(tptime[3]);
        t[0] += 1;
        tptime[3] = HexBcd(t[0]);
      }
      if (tptime[3] > time[4])  // tp_time[3]可能会是24H
        return n;
      if (tptime[3] == time[4]) {
        if ((tptime[4] > time[5]) || (tptime[4] == time[5])) return n;
      }
    }
  } else {
    while (1)  //非工作日处理
    {
      n += buf[13];
      t[0] = BcdHex(tptime[3]);
      t[0] += 1;
      tptime[3] = HexBcd(t[0]);
      if (tptime[3] > time[4])  // tp_time[3]可能会是24H
        return n;
      if (tptime[3] == time[4]) {
        if ((tptime[4] > time[5]) || (tptime[4] == time[5])) return n;
      }
    }
  }
}

uint ADD00TIME(void) {
  uint n = 0;
  uchar t[2];

  if (workday()) {
    while (1) {
      buf[100] = Timeslice2(tptime + 3);
      if ((buf[100] & 0x0f) == 1) {  //高峰时段收费
        n += buf[11];  //高峰时段第一小时后每半小时收费存放地址
        t[0] = BcdHex(tptime[3]);
        t[1] = BcdHex(tptime[4]);
        t[1] += 30;
        if (t[1] > 59) {
          t[1] -= 60;
          t[0] += 1;
          if (t[0] > 23) {
            tptime[4] = HexBcd(t[1]);
            tptime[3] = 0;
            DayAdd();
            return n;
          }
        }
        tptime[3] = HexBcd(t[0]);
        tptime[4] = HexBcd(t[1]);
      } else {  //非高峰时段收费
        n += buf[12];
        t[0] = BcdHex(tptime[3]);
        t[0] += 1;
        if (t[0] > 23) {
          tptime[3] = 0;
          DayAdd();
          return n;
        }
        tptime[3] = HexBcd(t[0]);
      }
    }
  } else {
    t[0] = BcdHex(tptime[3]);
    while (1)  //非工作日
    {
      n += buf[13];
      t[0] += 1;
      if (t[0] > 23) {
        tptime[3] = 0;
        DayAdd();
        return n;
      }
    }
  }
}

uint ADDONEDAY(void) {
  uint n;
  if (workday()) {
    n = ((buf[14] & 0x00ff) << 8) + (buf[15] & 0xff);
  } else {
    n = ((buf[16] & 0x00ff) << 8) + (buf[17] & 0xff);
  }
  DayAdd();
  return n;
}

uchar calweek(void) {
  uchar t[2], w;
  t[0] = BcdHex(tptime[3]);
  t[1] = BcdHex(tptime[4]);
  //出场的时间为23:59时,且出场的秒钟大于入场的秒钟,将停车时间减1分钟
  if ((time[6] > tptime[5]) && (time[4] == 0x23) && (time[5] == 0x59)) {
    if (stopmin == 0) {
      stopmin = 59;
      if (stophour == 0) {
        stophour = 23;
        stopday -= 1;
      } else {
        stophour -= 1;
      }
    } else {
      stopmin -= 1;
    }
  }
  //停车日时分+入场时分-->停车日时分
  stopmin += t[1];
  if (stopmin > 59) {
    stopmin -= 60;
    stophour += 1;
    if (stophour == 24) {
      stophour = 0;
      stopday += 1;
    }
  }
  stophour += t[0];
  if (stophour > 23) {
    stopday += 1;
  }

  w = time[0];
  while (stopday--) {
    if (w == 1)
      w = 7;
    else
      w -= 1;
  }
  return w;
}

/******************************************************************
*sfmode:收费模式0-a共10种
*sfcanshu: 根据卡类的收费模式从内存读取出来的32字节收费标准
*sta:=1，计算收费时无免费时间(如中央缴费过，再次缴费时，应该写1)，默认=0
*intime[]-入场时间  年月日时分秒
*outtime[]-出场时间 星期年月日时分秒

*sfmoney[]-返回收费金额
******************************************************************/
void CalMoney(uchar sfmode, uchar sfcanshu[], uchar sta, uchar intime[],
              uchar outtime[], uchar sfmoney[]) {
  uchar i, temp[2];
  uint flag, oneday;
  ulong m, t, p, k, n;
  uchar cstime;

  m = 0;
  mianfei = sta;
  Memcpy(block2 + 1, intime, 6);
  Memcpy(time, outtime, 7);
  if (ParkTime()) {
    Conzero(consume, 0, 3);
    Memcpy(sfmoney, consume, 3);
    return;
  }
  Memcpy(buf, sfcanshu, 32);
  if (mianfei) {
    buf[0] = 0;  //免费分钟写0
  }

  switch (sfmode) {
    case 0x01: {
      if ((stopday == 0) && (stophour == 0) && (!(stopmin > BcdHex(buf[0]))))
        break;
      oneday = ((buf[26] & 0x00ff) << 8) + (buf[27] & 0xff);
      if (stopmin == 0) {
        if (stophour == 0)
          m = (stopday * buf[25]);
        else
          m = (stopday * buf[25]) + buf[stophour];
      } else {
        m = (stopday * buf[25]) + buf[stophour + 1];
      }
      if (m > oneday) m = oneday;
      break;
    }
    case 0x02: {
      oneday = buf[12];                   //每天最高收费
      buf[100] = Timeslice3(block2 + 4);  //入场时间段数和所属时间段
      buf[101] = Timeslice3(time + 4);    //出场时间段数和所属时间段
      if (stopday > 0) {                  //超过1天时间处理
        m = oneday * stopday;
        if (buf[100] == buf[101]) {  //同时间段进出处理
          if ((buf[100] & 0x0f) == ((buf[100] >> 4) & 0x0f))
            temp[0] = BcdHex(buf[0]);  //取入场时间段的结束时
          else
            temp[0] = BcdHex(buf[(buf[100] & 0x0f) * 2]);
          i = BcdHex(block2[4]);
          temp[1] = 0;
          while (i != temp[0]) {
            temp[1] += 1;
            i += 1;
            if (i == 24) i = 0;
          }
          if (temp[1] < stophour) {           //中间跨段
            m += oneday;                      //取1天最高收费
          } else {                            //同段进同段出未跨段
            m += buf[(buf[100] & 0x0f) + 8];  //加本段收费
          }
        } else {
          if (Overtime(buf + 18))  //=1,超时
          {
            if (buf[30] & 0x08) {  //停车超过一天且已超时且跨段则强制最高值
              if (buf[100] != buf[101]) {
                m += oneday;
                buf[28] = (buf[30] >> 4) & 0x01;
                break;
              }
            }
            temp[0] = buf[28];  //处理方式选择
            temp[1] = buf[29];  //补偿值
          } else {
            temp[0] = buf[26];  //处理方式选择
            temp[1] = buf[27];  //补偿值
          }
          if (temp[0] & 0x01) {  //取补偿值
            m += temp[1];
          } else if (temp[0] & 0x02) {  //前段收费+补偿值
            if ((buf[101] & 0x0f) == 3) {
              m += buf[10];
              m += temp[1];
            } else if ((buf[101] & 0x0f) == 2) {
              m += buf[9];
              m += temp[1];
            } else {
              if (buf[4] == 0xff)  //判断有效段数
              {
                m += buf[10];
                m += temp[1];
              } else {
                m += buf[11];
                m += temp[1];
              }
            }
          } else if (temp[0] & 0x04) {  //本段收费+补偿值
            m += buf[(buf[101] & 0x0f) + 8];
            m += temp[1];
          } else if (temp[0] & 0x08) {  //最高收费段值+补偿值
            if (((buf[9] > buf[10]) || (buf[9] == buf[10])) &&
                ((buf[9] > buf[11]) || (buf[9] == buf[11]))) {
              m += buf[9];
              m += temp[1];
            } else if (((buf[10] > buf[9]) || (buf[10] == buf[9])) &&
                       ((buf[10] > buf[11]) || (buf[10] == buf[11]))) {
              m += buf[10];
              m += temp[1];
            } else {
              m += buf[11];
              m += temp[1];
            }
          } else {  //取每天最高收费
            m += oneday;
          }
        }

      } else {                       //未超过1天时间处理
        if (buf[100] == buf[101]) {  //同时间段进出处理
          if ((buf[100] & 0x0f) == ((buf[100] >> 4) & 0x0f))
            temp[0] = BcdHex(buf[0]);  //取入场时间段的结束时
          else
            temp[0] = BcdHex(buf[(buf[100] & 0x0f) * 2]);
          i = BcdHex(block2[4]);
          temp[1] = 0;  // temp[1]=该段的结束时-入场时
          while (i != temp[0]) {
            temp[1] += 1;
            i += 1;
            if (i == 24) i = 0;
          }
          if (temp[1] < stophour) {  //中间跨段
            m = oneday;              //取1天最高收费
          } else {                   //同段进同段出未跨段
            if (stophour < (buf[(buf[100] & 0x0f) + 5] / 60))
              break;
            else if (stophour == (buf[(buf[100] & 0x0f) + 5] / 60)) {
              if (stopmin < (buf[(buf[100] & 0x0f) + 5] % 60))
                break;
              else if (stopmin == (buf[(buf[100] & 0x0f) + 5] % 60))
                break;
            }
            if ((stophour < BcdHex(buf[13])) ||
                ((stophour == BcdHex(buf[13])) &&
                 (stopmin < BcdHex(buf[14])))) {  //未超时处理
              m = buf[(buf[100] & 0x0f) + 14];
            } else {  //超时处理
              m = buf[(buf[100] & 0x0f) + 8];
            }
          }
        } else {                 //跨段处理
          if (buf[30] & 0x01) {  //跨段免费继续有效
            if (stophour < (buf[(buf[100] & 0x0f) + 5] / 60))
              break;
            else if (stophour == (buf[(buf[100] & 0x0f) + 5] / 60)) {
              if (stopmin < (buf[(buf[100] & 0x0f) + 5] % 60))
                break;
              else if (stopmin == (buf[(buf[100] & 0x0f) + 5] % 60))
                break;
            }
          }

          if (buf[30] & 0x04) {  //各段累加后再按超时或未超时处理出场段
            temp[0] = buf[100] & 0x0f;
            while (temp[0] != (buf[101] & 0x0f)) {
              if (temp[0] == 1) {
                m += buf[9];
              } else if (temp[0] == 2) {
                m += buf[10];
              } else {
                m += buf[11];
              }
              if (temp[0] != ((buf[100] >> 4) & 0x0f))
                temp[0] += 1;
              else
                temp[0] = 1;
            }
          }
          if (Overtime(buf + 18))  //=1,超时
          {
            temp[0] = buf[22];  //处理方式选择
            temp[1] = buf[23];  //补偿值
          } else {
            temp[0] = buf[20];  //处理方式选择
            temp[1] = buf[21];  //补偿值
          }
          if (temp[0] & 0x01) {  //取补偿值
            m += temp[1];
          } else if (temp[0] & 0x02) {  //前段收费+补偿值
            if ((buf[101] & 0x0f) == 3) {
              m += buf[10];
              m += temp[1];
            } else if ((buf[101] & 0x0f) == 2) {
              m += buf[9];
              m += temp[1];
            } else {
              if (buf[4] == 0xff)  //判断有效段数
              {
                m += buf[10];
                m += temp[1];
              } else {
                m += buf[11];
                m += temp[1];
              }
            }
          } else if (temp[0] & 0x04) {  //本段收费+补偿值
            m += buf[(buf[101] & 0x0f) + 8];
            m += temp[1];
          } else if (temp[0] & 0x08) {  //最高收费段值+补偿值
            if (((buf[9] > buf[10]) || (buf[9] == buf[10])) &&
                ((buf[9] > buf[11]) || (buf[9] == buf[11]))) {
              m += buf[9];
              m += temp[1];
            } else if (((buf[10] > buf[9]) || (buf[10] == buf[9])) &&
                       ((buf[10] > buf[11]) || (buf[10] == buf[11]))) {
              m += buf[10];
              m += temp[1];
            } else {
              m += buf[11];
              m += temp[1];
            }
          } else {  //取每天最高收费
            m += oneday;
          }
        }
      }
      buf[28] = (buf[30] >> 4) & 0x01;
      break;
    }
    case 0x03:  //深圳收费标准
    {
      if ((stopday == 0)) {
        if (stophour == 0) {
          if ((stopmin < BcdHex(buf[0])) || (stopmin == BcdHex(buf[0]))) break;
        }
      }

      // Page_Read(buf+200,0x1FDE00,102);//??????
      Conzero(buf + 200, 0, 102);

      buf[26] = buf[18];  //最高收费转移到-->buff[26]
      buf[27] = buf[19];
      buf[28] = buf[21] & 0x01;
      if (buf[21] & 0x02) {  //按天收费
        if ((stophour != 0) || (stopmin != 0)) stopday += 1;
        m = stopday * buf[20];
      } else {  //按时段收费
        Memcpy(tptime, block2 + 1, 6);
        tptime[6] = calweek();

        if (workday()) {  //工作日
          buf[100] = Timeslice2(block2 + 4);
          if ((buf[100] & 0x0f) == 1) {  //高峰时段第一小时收费
            m = ((buf[5] & 0x00ff) << 8) + (buf[6] & 0xff);
          } else {  //非高峰时段第一小时收费
            m = ((buf[7] & 0x00ff) << 8) + (buf[8] & 0xff);
          }
        } else {  //非工作日第一小时收费
          m = ((buf[9] & 0x00ff) << 8) + (buf[10] & 0xff);
        }

        HourAdd();
        if (tptime[0] > time[1])  //出入场时间比较
          break;
        if (tptime[0] == time[1]) {
          if (tptime[1] > time[2]) break;
          if (tptime[1] == time[2]) {
            if (tptime[2] > time[3]) break;
            if (tptime[2] == time[3]) {
              if (tptime[3] > time[4]) break;
              if (tptime[3] == time[4]) {
                if (tptime[4] > time[5]) break;
                if (tptime[4] == time[5]) break;
              }
            }
          }
        }
        if (sameday()) {  //比较出入场时间是否为同一天
          m +=
              ADDENDTIME();  //入场时间与出场时间同一天，入场时间加1直到等于出场时间
        } else {
          m += ADD00TIME();  //入场时间与出场时间不同一天，入场时间加1直到00点
          while (1) {
            if (sameday()) {
              m += ADDENDTIME();
              break;
            } else {
              m += ADDONEDAY();
            }
          }
        }
      }
      break;
    }
    case 0x04:  //分段按半小时收费(北京)
    {
      temp[0] = buf[1];
      for (i = 1; i < 5; i++)  //弥补定义错误
      {
        buf[i] = buf[i + 1];
      }
      buf[5] = temp[0];
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }
      oneday = ((buf[16] & 0x00ff) << 8) + (buf[17] & 0xff);

      Memcpy(tptime, block2 + 1, 6);  // tp_time存放临时入场时间
      n = stophour * 2;               //把小时转换成半小时
      if ((stopmin > 30) || (stopmin == 30)) {
        n += 1;
        stopmin = stopmin - 30;
      }

      if (Timeslice2(tptime + 3) == 1) {
        temp[0] = buf[6];  //白天段首N个半小时
        temp[1] = buf[7];  //收费金额
      } else {
        temp[0] = buf[11];  //晚上段首N个半小时
        temp[1] = buf[12];  //收费金额
      }

      if ((n > temp[0]) || (n == temp[0])) {
        m = temp[1];
        n = n - temp[0];  //首N个半小时收费金额
        while (temp[0]--) {
          MinAdd(30);
        }
        if (Timeslice2(tptime + 3) == 1) {
          temp[0] = buf[8];  //白天段以后每M个半小时
          temp[1] = buf[9];  //收费金额
        } else {
          temp[0] = buf[13];  //晚上段以后每M个半小时
          temp[1] = buf[14];  //收费金额
        }

        while ((n > temp[0]) || (n == temp[0])) {
          n = n - temp[0];
          m += temp[1];  //以后每M个半小时收费金额
          while (temp[0]--) {
            MinAdd(30);
          }
          if (Timeslice2(tptime + 3) == 1) {
            temp[0] = buf[8];  //白天段以后每M个半小时
            temp[1] = buf[9];  //收费金额
          } else {
            temp[0] = buf[13];  //晚上段以后每M个半小时
            temp[1] = buf[14];  //收费金额
          }
        }

        if ((n * 30 + stopmin) > BcdHex(buf[10])) {
          m += temp[1];  //超时
        }
      } else {
        m = temp[1];  //首'N'个半小时收费金额
      }
      if (m > oneday) m = oneday;
      m = m + oneday * stopday;
      //当次最高收费
      n = ((buf[18] & 0x00ff) << 8) + (buf[19] & 0xff);
      if (m > n) m = n;
      buf[28] = buf[20] & 0x01;  //控制字
      break;
    }
    case 0x05:  //简易分段收费
    {
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }
      buf[100] = Timeslice2(block2 + 4);  //入场所属时间段
      buf[101] = Timeslice2(time + 4);    //出场所属时间段
      Twoslicetime(buf + 100, buf + 102);  //白天段停车时间-buff[102]/buff[103]
                                           //晚上段停车时间-buff[104]/buff[105]
      oneday = ((buf[7] & 0x00ff) << 8) + (buf[8] & 0xff);
      if (buf[15] & 0x40) {  //选择其它处理，则为惠州收费
        if (buf[100] == buf[101]) {
          flag = 0;
          if (buf[100] == 1) {
            if ((buf[104] != 0) || (buf[105] != 0)) flag = 1;
          } else {
            if ((buf[102] != 0) || (buf[103] != 0)) flag = 1;
          }
          if (flag) {  //=1，跨段
            m = buf[6];  //停车超过一天，剩余时间跨段取晚上段收费
          }              //停车时间超过3小时也取晚上段收费
          else           //=0，未跨段
          {
            if (stopday > 0) {  //停车超过一天未跨段取白天段收费
              m = buf[5];
            } else {
              if (buf[100] == 1) {
                m = buf[5];
              } else {
                m = buf[6];
              }
            }
          }
        } else {              //不同时间段进出处理
          if (stopday > 0) {  //停车超过一天跨段取晚上段收费
            m = buf[6];
          } else {
            if (stophour < 3)  //停车时间超过3小时也取晚上段收费
            {
              m = buf[5];
            } else {
              if ((stophour == 3) && (stopmin == 0)) {
                m = buf[5];
              } else {
                m = buf[6];
              }
            }
          }
        }
        if ((stophour == 0) && (stopmin == 0)) {
          m = 0;
        }
      } else if ((buf[15] & 0x01) || (buf[100] == buf[101])) {
        flag = 0;
        if ((buf[100] == buf[101]) && (!(buf[15] & 0x01))) {
          if (buf[100] == 1) {
            if ((buf[104] != 0) || (buf[105] != 0)) flag = 1;
          } else {
            if ((buf[102] != 0) || (buf[103] != 0)) flag = 1;
          }
        }
        if (flag) {  //取1天最高收费
          m = oneday;
        } else {
          if ((buf[102] != 0) || (buf[103] != 0)) {  //比较是否在N分钟内
            if ((buf[102] > BcdHex(buf[9])) || ((buf[102] == BcdHex(buf[9])) &&
                                                (buf[103] > BcdHex(buf[10])))) {
              m = buf[5];  //白天段收费总金额
            } else {
              m = buf[11];  //白天段N分钟内收费金额
            }
          }
          if ((buf[104] != 0) || (buf[105] != 0)) {  //比较是否在N分钟内
            if ((buf[104] > BcdHex(buf[12])) ||
                ((buf[104] == BcdHex(buf[12])) &&
                 (buf[105] > BcdHex(buf[13])))) {
              m += buf[6];  //晚上段收费总金额
            } else {
              m += buf[14];  //晚上段N分钟内收费金额
            }
          }
          if (m > oneday) {
            m = oneday;  //取1天最高收费
          }
        }
      } else {                 //不同时间段进出处理
        if (buf[15] & 0x10) {  //跨段时取高收费段收费
          m = buf[5];
          if (m < buf[6]) m = buf[6];
        } else if (buf[15] & 0x20) {  //跨段时取每天最高收费
          m = oneday;
        } else {
          if (buf[15] & 0x02) {  //跨段时收费按入场段处理
            flag = buf[100];
          } else if (buf[15] & 0x04) {  //跨段时收费按出场段处理
            flag = buf[101];
          } else {  //跨段时收费按高收费段处理
            if (buf[5] < buf[6])
              flag = 2;
            else
              flag = 1;
          }
          if (flag == 1) {
            if ((stophour > BcdHex(buf[9])) ||
                ((stophour == BcdHex(buf[9])) && (stopmin > BcdHex(buf[10])))) {
              if ((stophour > buf[120]) ||
                  ((stophour == buf[120]) && (stopmin > buf[121]))) {
                m = oneday;  //一天最高收费金额
              } else {
                m = buf[5];  //白天段收费总金额
              }
            } else {
              m = buf[11];  //白天段N分钟内收费金额
            }
          } else {
            if ((stophour > BcdHex(buf[12])) ||
                ((stophour == BcdHex(buf[12])) &&
                 (stopmin > BcdHex(buf[13])))) {
              if ((stophour > buf[122]) ||
                  ((stophour == buf[122]) && (stopmin > buf[123]))) {
                m = oneday;  //一天最高收费金额
              } else {
                m = buf[6];  //晚天段收费总金额
              }
            } else {
              m = buf[14];  //晚天段N分钟内收费金额
            }
          }
        }
      }
      m = m + oneday * stopday;
      buf[28] = buf[16] & 0x01;
      break;
    }
    case 0x06:  //分段按小时收费
    {
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }
      buf[100] = Timeslice2(block2 + 4);  //入场所属时间段
      buf[101] = Timeslice2(time + 4);    //出场所属时间段
      Twoslicetime(buf + 100, buf + 102);  //白天段停车时间-buff[102]/buff[103]
                                           //晚上段停车时间-buff[104]/buff[105]
      oneday = ((buf[14] & 0x00ff) << 8) + (buf[15] & 0xff);
      n = 0;

      if ((buf[102] != 0) || (buf[103] != 0)) {
        if ((buf[102] > BcdHex(buf[5])) ||
            ((buf[102] == BcdHex(buf[5])) &&
             (buf[103] > 0))) {  //白天段超过N小时收费金额
          m = buf[8];
        } else {
          flag = BcdHex(buf[6]);  //每M个小时
          while ((buf[102] > flag) || (buf[102] == flag)) {
            buf[102] = buf[102] - flag;
            m += buf[7];  //每M个小时收费金额K
          }
          if ((buf[102] != 0) || (buf[103] > BcdHex(buf[13]))) {
            m += buf[7];  //每M个小时收费金额K
          }
          if (m > buf[8]) {
            m = buf[8];  //取该段最高收费
          }
        }
      }

      if ((buf[104] != 0) || (buf[105] != 0)) {
        if ((buf[104] > BcdHex(buf[9])) ||
            ((buf[104] == BcdHex(buf[9])) &&
             (buf[105] > 0))) {  //晚上段超过N小时收费金额
          n = buf[12];
        } else {
          flag = BcdHex(buf[10]);  //每M个小时
          while ((buf[104] > flag) || (buf[104] == flag)) {
            buf[104] = buf[104] - flag;
            n += buf[11];  //每M个小时收费金额K
          }
          if ((buf[104] != 0) || (buf[105] > BcdHex(buf[13]))) {
            n += buf[11];  //每M个小时收费金额K
          }
          if (n > buf[12]) {
            n = buf[12];  //取该段最高收费
          }
        }
      }

      m += n;
      if (m > oneday) {
        m = oneday;
      }
      m = m + oneday * stopday;
      buf[28] = buf[17] & 0x01;
      break;
    }

    case 0x07:  //按半小时收费
    {
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }
      n = (stopday * 24 + stophour) * 2;  //把天转换成半小时
                                          //白天段首'N' 个半小时
      if ((stopmin > 30) || (stopmin == 30)) {
        n += 1;
        stopmin = stopmin - 30;
      }
      if ((n > buf[1]) || (n == buf[1])) {
        m = buf[2];
        n = n -
            buf[1];  //首N个半小时收费金额(设置时，如果N为0，对应的收费金额也写0)
        while ((n > buf[3]) || (n == buf[3])) {
          n = n - buf[3];
          m += buf[4];  //以后每R个半小时收费金额
        }
        if ((n * 30 + stopmin) > BcdHex(buf[5])) {
          m += buf[4];  //超时
        }
      } else {
        m = buf[2];  //首'N'个半小时收费金额
      }
      buf[28] = buf[6] & 0x01;
      break;
    }

    case 0x08:  //分段按半小时收费
    {
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }
      Memcpy(buf + 200, block2 + 1, 6);

      if (buf[22] & 0x04)  //先减免费分钟在计算费用
      {
        Memcpy(tptime, block2 + 1, 6);
        i = (buf[0] / 60) + ((BcdHex(tptime[4]) + (buf[0] % 60)) / 60);
        while (i--) {
          HourAdd();
        }
        tptime[4] = HexBcd((BcdHex(tptime[4]) + (buf[0] % 60)) % 60);
        Memcpy(block2 + 1, tptime, 6);
        if (ParkTime()) {
          Memcpy(block2 + 1, buf + 200, 6);
          break;
        }
      }
      if (buf[22] & 0x08)  //无分段，按白天段处理
      {
        buf[102] = stophour;
        buf[103] = stopmin;
        buf[104] = 0;
        buf[105] = 0;
      } else {
        buf[100] = Timeslice2(block2 + 4);  //入场所属时间段
        buf[101] = Timeslice2(time + 4);    //出场所属时间段
        Twoslicetime(buf + 100,
                     buf + 102);  //白天段停车时间-buff[102]/buff[103]
                                  //晚上段停车时间-buff[104]/buff[105]
      }
      Memcpy(block2 + 1, buf + 200, 6);

      oneday = ((buf[18] & 0x00ff) << 8) + (buf[19] & 0xff);
      t = 0;
      if ((buf[102] != 0) || (buf[103] != 0)) {
        n = buf[102] * 2;
        if ((buf[103] > 30) || (buf[103] == 30)) {
          n += 1;
          buf[103] = buf[103] - 30;
        }
        if ((stopday == 00) ||
            (!(buf[22] & 0x01))) {  //计算白天段前N个半小时收费M元
          if ((n > buf[5]) || (n == buf[5])) {
            m = buf[6];
            n = n - buf[5];
            while (((n > buf[7]) || (n == buf[7])) && (buf[7] != 0)) {
              n = n - buf[7];
              m += buf[8];  //以后每w个半小时收费金额
            }
            if ((n * 30 + buf[103]) > BcdHex(buf[17])) {
              m += buf[8];  //超时
            }
          } else {
            m = buf[6];
          }
        } else {
          while (((n > buf[7]) || (n == buf[7])) && (buf[7] != 0)) {
            n = n - buf[7];
            m += buf[8];  //以后每w个半小时收费金额
          }
          if ((n * 30 + buf[103]) > BcdHex(buf[17])) {
            m += buf[8];  //超时
          }
        }
        p = ((buf[9] & 0x00ff) << 8) + (buf[10] & 0xff);
        if (m > p) {
          m = p;
        }
      }

      if ((buf[104] != 0) || (buf[105] != 0)) {
        n = buf[104] * 2;
        if ((buf[105] > 30) || (buf[105] == 30)) {
          n += 1;
          buf[105] = buf[105] - 30;
        }
        if ((stopday == 00) ||
            (!(buf[22] & 0x02))) {  //计算晚上段前N个半小时收费M元
          if ((n > buf[11]) || (n == buf[11])) {
            t = buf[12];
            n = n - buf[11];
            while (((n > buf[13]) || (n == buf[13])) && (buf[13] != 0)) {
              n = n - buf[13];
              t += buf[14];  //以后每w个半小时收费金额
            }
            if ((n * 30 + buf[105]) > BcdHex(buf[17])) {
              t += buf[14];  //超时
            }
          } else {
            t = buf[12];
          }
        } else {
          while (((n > buf[13]) || (n == buf[13])) && (buf[13] != 0)) {
            n = n - buf[13];
            t += buf[14];  //以后每w个半小时收费金额
          }
          if ((n * 30 + buf[105]) > BcdHex(buf[17])) {
            t += buf[14];  //超时
          }
        }
        p = ((buf[15] & 0x00ff) << 8) + (buf[16] & 0xff);
        if (t > p) {
          t = p;
        }
      }
      m += t;
      if (m > oneday) {
        m = oneday;
      }
      m = m + oneday * stopday;
      p = ((buf[20] & 0x00ff) << 8) + (buf[21] & 0xff);  //当次最高收费
      if (m > p) {
        m = p;
      }
      buf[28] = buf[23] & 0x01;
      break;
    }

    case 0x09:  //分段按半小时收费
    {
      if ((stopday == 0)) {
        if (stophour < (buf[0] / 60))
          break;
        else if (stophour == (buf[0] / 60)) {
          if ((stopmin < (buf[0] % 60)) || (stopmin == (buf[0] % 60))) break;
        }
      }

      Memcpy(buf + 200, block2 + 1, 6);
      if ((buf[28] & 0x02) &&
          (!(buf[28] & 0x04)))  //总时间先减免费分钟在计算费用
      {
        Memcpy(tptime, block2 + 1, 6);
        i = (buf[0] / 60) + ((BcdHex(tptime[4]) + (buf[0] % 60)) / 60);
        while (i--) {
          HourAdd();
        }
        tptime[4] = HexBcd((BcdHex(tptime[4]) + (buf[0] % 60)) % 60);
        Memcpy(block2 + 1, tptime, 6);
        if (ParkTime()) {
          Memcpy(block2 + 1, buf + 200, 6);
          break;
        }
      }

      if (buf[28] & 0x01)  //无分段，按白天段处理
      {
        buf[102] = stophour;
        buf[103] = stopmin;
        buf[104] = 0;
        buf[105] = 0;
      } else {
        buf[100] = Timeslice2(block2 + 4);  //入场所属时间段
        buf[101] = Timeslice2(time + 4);    //出场所属时间段
        Twoslicetime(buf + 100,
                     buf + 102);  //白天段停车时间-buff[102]/buff[103]
                                  //晚上段停车时间-buff[104]/buff[105]
      }
      oneday = ((buf[26] & 0x00ff) << 8) + (buf[27] & 0xff);
      t = 0;

      if ((!(buf[28] & 0x10)) || (buf[100] != buf[101]) ||
          ((buf[102] == 0) && (buf[103] == 0)) ||
          ((buf[104] == 0) &&
           (buf[105] == 0))) {  //跨段时将两段停留时间分别计算，并将计算结果相加
        m = Calfeel(buf + 102, buf + 5, buf[25], buf[28],
                    buf[0]);  //计算白天段收费金额
        t = Calfeel(buf + 104, buf + 15, buf[25], buf[28],
                    buf[0]);  //计算晚上段收费金额
        m += t;
        if (m > oneday) m = oneday;
      } else {  //跨段时按停车时间逐段计算，最后将计算结果相加
        Threeslicetime(buf[100], buf + 106, block2 + 4, time + 4);
        // buff[106],buff[107]存放入场时间到该段的结束时间之间停留停车时间
        // buff[108],buff[109]存放该段的起始时间到出场时间之间停留停车时间，
        if (buf[100] == 1) {
          m = Calfeel(buf + 106, buf + 5, buf[25], buf[28],
                      buf[0]);  //计算入场时间到该段的结束时间之间收费金额
          t = Calfeel(buf + 108, buf + 5, buf[25], buf[28],
                      buf[0]);  //计算该段的起始时间到出场时间之间收费金额
          m += t;
          m += (((buf[23] & 0x00ff) << 8) +
                (buf[24] & 0xff));  //晚上段最高收费金额
        } else {
          m = Calfeel(buf + 106, buf + 15, buf[25], buf[28],
                      buf[0]);  //计算入场时间到该段的结束时间之间收费金额
          t = Calfeel(buf + 108, buf + 15, buf[25], buf[28],
                      buf[0]);  //计算该段的起始时间到出场时间之间收费金额
          m += t;
          m += (((buf[13] & 0x00ff) << 8) +
                (buf[14] & 0xff));  //白天段最高收费金额
        }
      }
      Memcpy(block2 + 1, buf + 200, 6);

      m = m + oneday * stopday;
      buf[28] = (buf[29] & 0x01);
      break;
    }

    case 0x0a:  //按15分钟为单位计算
    {
      if ((buf[29] & 0x01) || (!(buf[29] & 0x02)) ||
          (!(buf[29] &
             0x08))) {  //只有一个免费分钟或停车按白天段处理(免费分钟为白天段的免费分钟)
        if ((stopday ==
             0))  //或分段方式选择：跨段时如本段时间不足整数，则由下段补足时间
        {
          if (stophour < (buf[5] / 60))
            break;
          else if (stophour == (buf[5] / 60)) {
            if ((stopmin < (buf[5] % 60)) || (stopmin == (buf[5] % 60))) break;
          }
        }

        Memcpy(buf + 200, block2 + 1, 6);
        if (buf[29] & 0x04) {  //计算收费时先减免费分钟
          Memcpy(tptime, block2 + 1, 6);
          i = (buf[5] / 60) + ((BcdHex(tptime[4]) + (buf[5] % 60)) / 60);
          while (i--) {
            HourAdd();
          }
          tptime[4] = HexBcd((BcdHex(tptime[4]) + (buf[5] % 60)) % 60);
          Memcpy(block2 + 1, tptime, 6);
          if (ParkTime()) {
            Memcpy(block2 + 1, buf + 200, 6);
            break;
          }
        }
      } else {
        Memcpy(buf + 200, block2 + 1, 6);
      }

      oneday = ((buf[25] & 0x00ff) << 8) + (buf[26] & 0xff);
      t = 0;
      if ((!(buf[29] & 0x08)) &&
          (!(buf[29] & 0x01))) {  //跨段时如本段时间不足整数，则由下段补足时间
        Memcpy(tptime, block2 + 1, 6);    // tp_time存放临时入场时间
        n = stophour * 4 + stopmin / 15;  //把小时转换成N个15分钟
        stopmin = stopmin % 15;

        if (Timeslice2(tptime + 3) == 1) {
          if ((stopday != 0) && (buf[29] & 0x40)) {
            temp[0] = 0;  //停车时间超过1天，白天段无前N小时收费
            p = 0;
          } else {
            temp[0] = buf[6];           //白天段首N个15分钟
            p = buf[7] * 256 + buf[8];  //收费金额
          }
        } else {
          if ((stopday != 0) && (buf[29] & 0x80)) {
            temp[0] = 0;  //停车时间超过1天，晚上段无前N小时收费
            p = 0;
          } else {
            temp[0] = buf[16];            //晚上段首N个15分钟
            p = buf[17] * 256 + buf[18];  //收费金额
          }
        }

        if (buf[30] & 0x10) {
          cstime = buf[12];  //超时时间取16进制
        } else {
          cstime = BcdHex(buf[12]);  //超时时间取10进制
        }
        // 2018.07.27修改,如果时间小于或等于超时时间不计费
        if (((n > temp[0]) || (n == temp[0])) &&
            ((n * 15 + stopmin) > cstime)) {
          m = p;
          n = n - temp[0];  //首N个15分钟收费金额
          while (temp[0]--) {
            MinAdd(15);
          }
          if (Timeslice2(tptime + 3) == 1) {
            temp[0] = buf[9];             //白天段以后每M个15分钟
            p = buf[10] * 256 + buf[11];  //收费金额
          } else {
            temp[0] = buf[19];            //晚上段以后每M个15分钟
            p = buf[20] * 256 + buf[21];  //收费金额
          }
          // 2018.07.27修改,如果时间小于或等于超时时间不计费
          while (((n > temp[0]) || (n == temp[0])) && temp[0] &&
                 ((n * 15 + stopmin) > cstime)) {
            m += p;  //以后每M个15分钟收费金额
            n = n - temp[0];
            while (temp[0]--) {
              MinAdd(15);
            }
            if (Timeslice2(tptime + 3) == 1) {
              temp[0] = buf[9];  //白天段以后每M个15分钟
              p = buf[10] * 256 + buf[11];
            } else {
              temp[0] = buf[19];  //晚上段以后每M个15分钟
              p = buf[20] * 256 + buf[21];
            }
          }

          if ((n * 15 + stopmin) >
              cstime)  // 2018.07.27修改,如果时间小于或等于超时时间不计费
          {
            m += p;  //超时
          }
        } else {
          // 2018.07.27修改,如果时间小于或等于超时时间不计费
          if (((n != 0) || (stopmin != 0)) && ((n * 15 + stopmin) > cstime))
            m = p;  //首'N'个15分钟收费金额
        }
        if (m > oneday) m = oneday;
      } else {
        if ((stopday != 0) && (buf[29] & 0x40)) {
          temp[0] =
              1;  //超过1天后，剩余不足1天时间白天段无前N小时收费，直接按每M个15分钟收费
        } else {
          temp[0] = 0;
        }
        if ((stopday != 0) && (buf[29] & 0x80)) {
          temp[1] =
              1;  //超过1天后，剩余不足1天时间晚上段无前N小时收费，直接按每M个15分钟收费
        } else {
          temp[1] = 0;
        }
        if (buf[29] & 0x01) {  //无分段按白天段处理
          buf[102] = stophour;
          buf[103] = stopmin;
          buf[104] = 0;
          buf[105] = 0;
        } else {
          buf[100] = Timeslice2(block2 + 4);  //入场所属时间段
          buf[101] = Timeslice2(time + 4);    //出场所属时间段
          Twoslicetime(buf + 100,
                       buf + 102);  //白天段停车时间-buff[102]/buff[103]
                                    //晚上段停车时间-buff[104]/buff[105]
        }
        if ((buf[29] & 0x01) || (!(buf[29] & 0x10)) || (buf[100] != buf[101]) ||
            ((buf[102] == 0) && (buf[103] == 0) &&
             ((stopday == 0) || ((buf[29] & 0x38) == 0x38))) ||
            ((buf[104] == 0) && (buf[105] == 0) &&
             ((stopday == 0) ||
              ((buf[29] & 0x38) ==
               0x38)))) {  //跨段时将两段停留时间分别计算，并将计算结果相加
          m = CalfeelA(buf + 102, buf + 5, buf[29], buf[30],
                       temp[0]);  //计算白天段收费金额
          t = CalfeelA(buf + 104, buf + 15, buf[29], buf[30],
                       temp[1]);  //计算晚上段收费金额
          m += t;
          if (m > oneday) m = oneday;
        } else {  //(无一天最高收费原则，按时间段逐段计算并累加费用)跨段时按停车时间逐段计算，最后将计算结果相加(同段进同段出处理)
          if (((buf[102] == 0) && (buf[103] == 0)) ||
              ((buf[104] == 0) && (buf[105] == 0))) {
            stopday -= 1;
          }
          Threeslicetime(buf[100], buf + 106, block2 + 4, time + 4);
          // buff[106],buff[107]存放入场时间到该段的结束时间之间停留停车时间
          // buff[108],buff[109]存放该段的起始时间到出场时间之间停留停车时间，
          if (buf[100] == 1) {
            m = CalfeelA(buf + 106, buf + 5, buf[29], buf[30],
                         temp[0]);  //计算入场时间到该段的结束时间之间收费金额
            t = CalfeelA(buf + 108, buf + 5, buf[29], buf[30],
                         temp[0]);  //计算该段的起始时间到出场时间之间收费金额
            m += t;
            m += (((buf[23] & 0x00ff) << 8) +
                  (buf[24] & 0xff));  //晚上段最高收费金额
          } else {
            m = CalfeelA(buf + 106, buf + 15, buf[29], buf[30],
                         temp[1]);  //计算入场时间到该段的结束时间之间收费金额
            t = CalfeelA(buf + 108, buf + 15, buf[29], buf[30],
                         temp[1]);  //计算该段的起始时间到出场时间之间收费金额
            m += t;
            m += (((buf[13] & 0x00ff) << 8) +
                  (buf[14] & 0xff));  //白天段最高收费金额
          }
        }
      }
      Memcpy(block2 + 1, buf + 200, 6);
      m = m + (oneday * stopday);
      buf[28] = (buf[30] & 0x0f);
      break;
    }

    default:
      break;
  }

  if (m < 1000000) {
    t = m % 10000;
    p = t % 1000;
    k = p % 100;
    n = k % 10;
    consume[0] = HexBcd(m / 10000);
    consume[1] = ((t / 1000) << 4) + (p / 100);
    consume[2] = ((k / 10) << 4) + n;

    if ((buf[28] & 0x03) == 0) {  //以元为单位
      if (consume[0] == 0) {
        consume[0] = consume[1];
        consume[1] = consume[2];
        consume[2] = 0;
      } else {
        consume[0] = 0x99;
        consume[1] = 0x99;
        consume[2] = 0x99;
      }
    } else if ((buf[28] & 0x03) == 0x01) {  //以角为单位
      if ((consume[0] & 0xf0) == 0) {
        consume[0] = ((consume[0] << 4) & 0xf0) + ((consume[1] >> 4) & 0x0f);
        consume[1] = ((consume[1] << 4) & 0xf0) + ((consume[2] >> 4) & 0x0f);
        consume[2] = ((consume[2] << 4) & 0xf0);
      } else {
        consume[0] = 0x99;
        consume[1] = 0x99;
        consume[2] = 0x99;
      }
    }
  } else {
    consume[0] = 0x99;
    consume[1] = 0x99;
    consume[2] = 0x99;
  }

  if (buf[28] & 0x08) {  //收费金额精确到元
    consume[2] = 0;
  } else if (buf[28] & 0x04) {  //收费金额精确到角
    consume[2] &= 0xf0;
  }
  Memcpy(sfmoney, consume, 3);
}

/************************************************************
*sfmode:收费模式0-a共10种
*sfcanshu: 根据卡类的收费模式从内存读取出来的32字节收费标准
*mian[]-免费天时分3字节16进制,如果全免mian[0]=0xa0,mian[1]=0,mian[2]=0;
*免10小时mian[0]=0x00,mian[1]=0x0a,mian[2]=0;
*sta状态：=1,表示第一次缴费；=n,表示第n次缴费(除了第一次缴费有免费时间外，其它无免费时间，只判断超时时间
*flag：flag.0=1,刷POS机后免前n小时，=0，免后n小时
       flag.1=1,刷POS免后N小时时，入场时间往后移N小时计算收费；=0，刷POS免后N小时时，出场时间往前移N小时计算收费；
           flag.2=1,刷POS机后超时部分计费时，免费分钟继续有效；=0，刷POS机后超时部分计费无免费分钟

*intime[]-入场时间 年月日时分秒6字节
*outtime[]-出场时间 星期年月日时分秒7字节

*sfmoney[]-返回收费金额
*mfmoney[]-返回免费金额
************************************************************/
void Softcal(uchar sfmode, uchar sfcanshu[], uchar mian[], uchar sta,
             uchar flag, uchar intime[], uchar outtime[], uchar sfmoney[],
             uchar mfmoney[]) {
  uchar str[7], cons[3], yueka, i;
  uint Period;

  Memcpy(block2 + 7, mian, 3);
  Memcpy(block2 + 1, intime, 6);
  Memcpy(time, outtime, 7);
  if (sta)
    mianfei = 0;
  else
    mianfei = 1;

  Conzero(tmpmoney, 0, 3);
  if (block2[7] == 0xa0) {
    if (ParkTime()) {
      Conzero(consume, 0, 3);
    } else {
      CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
      Memcpy(consume, sfmoney, 3);
      Memcpy(tmpmoney, consume, 3);
      Conzero(consume, 0, 3);
    }
  } else {
    if (flag & 0x01) {                //免费前N小时
      Memcpy(str, time, 7);           //暂存出场时间
      Memcpy(tptime, block2 + 1, 6);  //暂存入场时间
      if (ParkTime()) {
        tptime[6] = time[0];
      } else {
        tptime[6] = calweek();
      }
      tmpmoney[0] = BcdHex(block2[7]);
      tmpmoney[1] = BcdHex(block2[8]);
      tmpmoney[2] = BcdHex(block2[9]);
      stopday = tmpmoney[0];
      stophour = tmpmoney[1];
      stopmin = tmpmoney[2];
      while (tmpmoney[2]--) {
        MinuteAdd();
      }
      while (tmpmoney[1]--) {
        HourAdd();
      }
      while (tmpmoney[0]--) {
        DayAdd();
      }
      Memcpy(time + 1, tptime, 6);
      time[0] = tptime[6];
      CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
      Memcpy(consume, sfmoney, 3);
      Memcpy(tmpmoney, consume, 3);
      Memcpy(time, str, 7);
      if (ParkTime()) {
        Conzero(consume, 0, 3);
      } else {
        CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
        Memcpy(consume, sfmoney, 3);
      }

      if (Concmp(consume, 0x99, 3)) {  //如果收费金额过大，则不处理
        if (Arrycmp(consume, tmpmoney, 3)) {
          Memcpy(tmpmoney, consume, 3);
          Conzero(consume, 0, 3);
        } else {
          yueka = 0;
          for (i = 3; i > 0; i--) {
            Period = ((0x9a - tmpmoney[i - 1] - yueka) & 0x0f) +
                     (consume[i - 1] & 0x0f);
            if (Period > 9) Period += 6;
            Period += ((0x9a - tmpmoney[i - 1] - yueka) & 0xf0) +
                      (consume[i - 1] & 0xf0);
            if ((Period >> 4) > 9) {
              Period += 0x60;
              yueka = 0;
            } else {
              yueka = 1;
            }
            consume[i - 1] = (Period & 0xff);
          }
        }
      }
    } else {           //减免后N小时
      if (ParkTime())  //减免前应该收费金额
      {
        Conzero(consume, 0, 3);
      } else {
        CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
        Memcpy(consume, sfmoney, 3);
      }
      Memcpy(cons, consume, 3);

      if (Concmp(consume, 0x99, 3)) {  //如果收费金额过大，则不处理
        //减免后几小时
        if (flag & 0x02) {                //入场时间往后移N小时
          Memcpy(str, block2 + 1, 6);     //暂存入场时间
          Memcpy(tptime, block2 + 1, 6);  //暂存入场时间
          tmpmoney[0] = BcdHex(block2[7]);
          tmpmoney[1] = BcdHex(block2[8]);
          tmpmoney[2] = BcdHex(block2[9]);
          while (tmpmoney[2]--) {
            MinuteAdd();
          }
          while (tmpmoney[1]--) {
            HourAdd();
          }
          while (tmpmoney[0]--) {
            DayAdd();
          }
          Memcpy(block2 + 1, tptime, 6);
          if (ParkTime()) {
            Conzero(consume, 0, 3);
          } else {
            if (flag & 0x04) {
              mianfei = 0;
            } else {
              mianfei = 1;  //计算收费时无免费时间
            }
            CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
            Memcpy(consume, sfmoney, 3);
            mianfei = 0;
          }
          Memcpy(block2 + 1, str, 6);
        } else {                        //出场时间往前移N小时
          Memcpy(str, time, 7);         //暂存出场时间
          Memcpy(tptime, time + 1, 6);  //暂存入场时间
          tptime[6] = time[0];
          tmpmoney[0] = BcdHex(block2[7]);
          tmpmoney[1] = BcdHex(block2[8]);
          tmpmoney[2] = BcdHex(block2[9]);
          while (tmpmoney[2]--) {
            MinuteSub();
          }
          while (tmpmoney[1]--) {
            HourSub();
          }
          while (tmpmoney[0]--) {
            DaySub();
          }
          Memcpy(time + 1, tptime, 6);
          time[0] = tptime[6];
          if (ParkTime()) {
            Conzero(consume, 0, 3);
          } else {
            if (flag & 0x04) {
              mianfei = 0;
            } else {
              mianfei = 1;  //计算收费时无免费时间
            }
            CalMoney(sfmode, sfcanshu, mianfei, block2 + 1, time, sfmoney);
            Memcpy(consume, sfmoney, 3);
            mianfei = 0;
          }
          Memcpy(time, str, 7);
        }

        //////
        if (!Arrycmp(cons, consume, 3)) {
          yueka = 0;
          for (i = 3; i > 0; i--) {
            Period =
                ((0x9a - consume[i - 1] - yueka) & 0x0f) + (cons[i - 1] & 0x0f);
            if (Period > 9) Period += 6;
            Period +=
                ((0x9a - consume[i - 1] - yueka) & 0xf0) + (cons[i - 1] & 0xf0);
            if ((Period >> 4) > 9) {
              Period += 0x60;
              yueka = 0;
            } else {
              yueka = 1;
            }
            cons[i - 1] = (Period & 0xff);
          }
          Memcpy(tmpmoney, cons, 3);
        } else {
          Conzero(tmpmoney, 0, 3);
        }
      }
    }
  }
  Memcpy(sfmoney, consume, 3);
  Memcpy(mfmoney, tmpmoney, 3);
}
}  // namespace charge
}  // namespace camera
